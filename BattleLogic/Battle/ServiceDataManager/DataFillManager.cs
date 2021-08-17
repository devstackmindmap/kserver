using AkaEnum.Battle;
using CommonProtocol;
using System.Collections.Generic;

namespace BattleLogic
{
    public abstract class DataFillManager
    {
        private Battle _battle;
        protected Battle Battle => _battle;

        public void SetBattle(Battle battle)
        {
            _battle = battle;
        }
        
        protected abstract void FillCardInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts);

        protected virtual void FillEnemyCardInfo(PlayerType playerType, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
        }

        public void FillBattleStartInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStart)
        {
            protoBattleStart[PlayerType.Player1].MyPlayerType = PlayerType.Player1;
            protoBattleStart[PlayerType.Player2].MyPlayerType = PlayerType.Player2;

            FillCardInfo(protoBattleStart);

            Battle.Players[PlayerType.Player1].FillEnemyInfo(protoBattleStart[PlayerType.Player1]);
            Battle.Players[PlayerType.Player2].FillEnemyInfo(protoBattleStart[PlayerType.Player2]);
        }

        public void FillCurrentBattleStatus(PlayerType playerType, BattleType battleType, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            protoCurrentBattleStatus.BattleType = battleType;
            protoCurrentBattleStatus.MyPlayerType = playerType;
            protoCurrentBattleStatus.AccumulatedBulletTime = Battle.Status.AccumulatedBulletTime;
            protoCurrentBattleStatus.BattleStartTime = Battle.Status.StartDateTime.Ticks;

            var handCards = Battle.BattleHelper.GetHandCards(playerType);

            protoCurrentBattleStatus.HandCardStatIds = new Dictionary<int, uint>();
            protoCurrentBattleStatus.HandCardDecreaseElixir = new Dictionary<int, int>();
            foreach (var handCard in handCards)
            {
                protoCurrentBattleStatus.HandCardStatIds.Add(handCard.Key, handCard.Value.CardStatId);
                protoCurrentBattleStatus.HandCardDecreaseElixir.Add(handCard.Key, handCard.Value.DecreaseElixir);
            }

            protoCurrentBattleStatus.NextCardStatId = Battle.BattleHelper.GetNexCardStatId(playerType);

            Battle.Players[playerType].FillUnitInfo(protoCurrentBattleStatus.Units);
            Battle.Players[playerType].FillEnemyInfo(protoCurrentBattleStatus);
            FillEnemyCardInfo(playerType, protoCurrentBattleStatus);

            //TODO passiveInfo
        }

        public void FillUpdateCardInfo(PlayerType playerType, ProtoUnitDeath protoUnitDeath)
        {
            protoUnitDeath.HandCardStatIds = GetHandCardStatIds(playerType);
            protoUnitDeath.NextCardStatId = Battle.BattleHelper.GetNexCardStatId(playerType);
        }

        protected Dictionary<int, uint> GetHandCardStatIds(PlayerType playerType)
        {
            var cardStatIds = new Dictionary<int, uint>();
            var handCards = Battle.BattleHelper.GetHandCards(playerType);

            foreach (var handCard in handCards)
            {
                cardStatIds.Add(handCard.Key, handCard.Value.CardStatId);
            }
            return cardStatIds;
        }

        protected List<uint> GetAllCards(PlayerType playerType, Dictionary<int, uint> cardStatIds)
        {
            var retCardStatIds = new List<uint>();

            foreach (var cardStatId in cardStatIds)
            {
                retCardStatIds.Add(cardStatId.Value);
            }

            var waitCards = _battle.BattleHelper.GetWaitCards(playerType);
            foreach (var card in waitCards)
            {
                retCardStatIds.Add(card.CardStatId);
            }

            return retCardStatIds;
        }
    }
}
