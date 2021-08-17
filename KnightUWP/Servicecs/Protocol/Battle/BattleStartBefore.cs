using AkaData;
using AkaDB.MySql;
using AkaEnum.Battle;
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
    [Message(MessageType = MessageType.BeforeBattleStart, Name = "BeforeStart" )]
    public class BattleStartBefore : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoBeforeBattleStart>.Deserialize(data);
        }

        public override void OnResponse<TContext>(TContext context, BaseProtocol _)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoBeforeBattleStart;

            userInfo.CurrentBattleInfo.BeforeBattleStartInfo = protocol;

            userInfo.CurrentBattleInfo.MyPlayer = userInfo.CurrentBattleInfo.BeforeBattleStartInfo.MyPlayerType;
            userInfo.CurrentBattleInfo.EnemyPlayer = userInfo.CurrentBattleInfo.MyPlayer == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;


            userInfo.CurrentBattleInfo.MyUnits.Clear();
            userInfo.CurrentBattleInfo.EnemyUnits.Clear();

            uint cardStatId = 0;
            if (protocol.HandCardStatIds.TryGetValue(0, out cardStatId))
                userInfo.CurrentBattleInfo.Card1 = cardStatId;
            if (protocol.HandCardStatIds.TryGetValue(1, out cardStatId))
                userInfo.CurrentBattleInfo.Card2 = cardStatId;
            if (protocol.HandCardStatIds.TryGetValue(2, out cardStatId))
                userInfo.CurrentBattleInfo.Card3 = cardStatId;
            if (protocol.HandCardStatIds.TryGetValue(3, out cardStatId))
                userInfo.CurrentBattleInfo.Card4 = cardStatId;
            var deckInfo = userInfo.CurrentMyBattleDeck.UserAndDecks[userInfo.users.userId];
            
            var units = deckInfo.Deck.DeckElements.Where(element => element.SlotType == AkaEnum.SlotType.Unit)
                                                                      .OrderBy(unit => unit.OrderNum)
                                                                      .ToArray();
            for (int i = 0; i < units.Length; i++)
            {
                var unitInfo = new UnitInfo();
                var unitData = Data.GetUnit(units[i].ClassId);
                var unitLevel = deckInfo.UnitsInfo[i].Level;
                var unitSkinId = deckInfo.UnitsInfo[i].SkinId;
                var skin = unitSkinId == 0 ? unitData.UnitInitial + "_Basic" : Data.GetDataSkin(unitSkinId).SheetName;

                var unitStat = Data.GetUnitStat(unitData.UnitId, unitLevel);

                if (ResourceManager.Images.TryGetValue(unitData.UnitInitial, out var image))
                    unitInfo.Image = image;
                if (unitStat != null)
                {
                    unitInfo.Hp = unitStat.Hp;
                    unitInfo.Level = unitStat.Level;
                    unitInfo.Skin = skin;
                }

                unitInfo.Name = unitData.UnitInitial;
                unitInfo.UnitId = unitData.UnitId;
                userInfo.CurrentBattleInfo.MyUnits.Add(unitInfo);                
            }
            

            var eUnits = protocol.EnemyPlayer.Units;
            for (int i =0; i< eUnits.Count; i++)
            {
                var unitInfo = new UnitInfo();
                var unitData = Data.GetUnit(eUnits[i].UnitId);
                var unitLevel = eUnits[i].Level;
                var unitSkinId = eUnits[i].SkinId;
                var skin = unitSkinId == 0 ? unitData.UnitInitial + "_Basic" : Data.GetDataSkin(unitSkinId).SheetName;

                var unitStat = Data.GetUnitStat(unitData.UnitId, unitLevel);

                if (ResourceManager.Images.TryGetValue(unitData.UnitInitial, out var image))
                    unitInfo.Image = image;
                if (unitStat != null)
                {
                    unitInfo.Hp = unitStat.Hp;
                    unitInfo.Level = unitStat.Level;
                    unitInfo.Skin = skin;
                }

                unitInfo.Name = unitData.UnitInitial;
                unitInfo.UnitId = unitData.UnitId;
                userInfo.CurrentBattleInfo.EnemyUnits.Add(unitInfo);
              

            }
            
            



            //TODO
            //card setting
            //round setting
            //unit setting
            //enemy setting

            userInfo.State = UserState.BattleBeforeStart;

            userInfo.Latencties.Clear();
            userInfo.LastSyncTimeSended = DateTime.UtcNow;
            userInfo.ReceivedSyncTime = true;
            userInfo.MaxLatency = 0;

            Net.Battle.SendReady(userInfo);
        }

        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoBeforeBattleStart;
            return $"{userInfo.accounts.userId}:{userInfo.accounts.nickName}-{protocol.MyPlayerType} vs {protocol.EnemyPlayer.UserId}:{protocol.EnemyPlayer.Nickname}-{(protocol.MyPlayerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1)}";
        }
    }
}
