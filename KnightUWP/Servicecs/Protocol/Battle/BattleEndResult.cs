using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Protocol
{
    [Message(MessageType = MessageType.GetBattleResultKnightLeague, Name ="BattleResult")]
    public class BattleEndResult : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoOnBattleResult>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol resData)
        {
            var userInfo = context as UserInfo;
            var endResult = resData as ProtoOnBattleResultRank;


            //   userInfo.BeforeBattleStartInfo = AkaSerializer<ProtoOnBattleResult>.Deserialize(data);


            userInfo.State = UserState.None;
            userInfo.accounts.currentSeasonRankPoint = endResult.UserRankData.CurrentSeasonRankPoint;
            userInfo.accounts.maxRankLevel = endResult.UserRankData.MaxRankLevel;
            userInfo.users.level = endResult.UserLevelAndExp.Level;

            userInfo.CurrentBattleInfo.BattleEndTime = DateTime.UtcNow;
            userInfo.CurrentBattleInfo.BattleTime = (userInfo.CurrentBattleInfo.BattleEndTime - userInfo.CurrentBattleInfo.BattleStartTime).TotalSeconds.ToString();
            userInfo.CurrentBattleInfo.ElixirRelease();


            var homePage = UX.Frame.RootFrame.MainFrame.CurrentPage as UX.Pages.HomePage;
            if (homePage != null)
            {
                homePage.ViewModelProvider.IncreaseTotalEndBattleCount();
            }

            userInfo.BattleConnecter?.Close();
            userInfo.WriteLog();
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoOnBattleResult;
            return $"Result:{protocol.BattleResultType}";
        }


        protected override string GetCenter(BaseProtocol protocol)
        {
            var result = protocol as ProtoOnBattleResult;
            return result.BattleResultType.ToString();
        }
    }
}
