using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs
{
    public sealed partial class VOProvider
    {

        private async Task WaitState(UserInfo userInfo, int maxCount, params UserState[] state)
        {
            while (false == userInfo.State.In(state) && maxCount-- > 0)
                await Task.Delay(1);
        }


        public void SendCardUse(UserInfo userInfo)
        {
            Net.Battle.SendCardUse(userInfo);
        }

        public void SendSyncTime(UserInfo userInfo)
        {
            userInfo.LastSyncTimeSended = DateTime.UtcNow;
            userInfo.ReceivedSyncTime = false;

            //   Utility.Log($"Send {userInfo.LastSyncTimeSended.ToString("ss.fff")} ");
            Net.Battle.SendSyncTime(userInfo);
        }

        public async Task MatchingAndBattleStart(UserInfo userInfo)
        {
            if (userInfo.users.userId == 0)
                return;


            userInfo.CurrentMyBattleDeck = await Net.API.GetDeckWithNum(userInfo, ModeType.PVP);
            if (userInfo.CurrentMyBattleDeck.UserAndDecks.TryGetValue(userInfo.users.userId, out var deckInfo) == false
                || deckInfo.UnitsInfo.Count != 3
                || deckInfo.CardsLevel.Count != 8)
            {
                return;
            }


            IncreaseTotalTryMatchingCount();

            await Net.Match.TryMatching(userInfo);

            await WaitState(userInfo,60000, UserState.Matched, UserState.None);
            Utility.Log($"Match Result {userInfo.State.ToString()}");
            if (userInfo.State == UserState.Matched)
            {
                if (EnableMatchingTest == true)
                {
                    userInfo.State = UserState.None;
                    return;
                }
                Net.Battle.EnterRoom(userInfo);
            }
            else
            {
                //Match failed
                userInfo.State = UserState.None;
                IncreaseTotalTryMatchingFailCount();
                userInfo.WriteLog("MatchingFail");
            }
        }

    }
}
