using AkaEnum.Battle;
using CommonProtocol;
using System.Collections.Generic;

namespace BattleLogic
{
    public sealed class HiDataFillManager : DataFillManager
    {
        protected override void FillEnemyCardInfo(PlayerType playerType, ProtoCurrentBattleStatus protoCurrentBattleStatus)
        {
            var enemyPlayerType = GetEnemyPlayerType(playerType);

            var handCards = Battle.BattleHelper.GetHandCards(enemyPlayerType);
            var waitCards = Battle.BattleHelper.GetWaitCards(enemyPlayerType);
            foreach (var card in handCards)
            {
                protoCurrentBattleStatus.EnemyPlayer.CardStatIds.Add(card.Value.CardStatId);
            }

            foreach (var card in waitCards)
            {
                protoCurrentBattleStatus.EnemyPlayer.CardStatIds.Add(card.CardStatId);
            }
        }

        protected override void FillCardInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            protoBattleStarts[PlayerType.Player1].HandCardStatIds = GetHandCardStatIds(PlayerType.Player1);
            protoBattleStarts[PlayerType.Player2].HandCardStatIds = GetHandCardStatIds(PlayerType.Player2);

            protoBattleStarts[PlayerType.Player1].NextCardStatId = Battle.BattleHelper.GetNexCardStatId(PlayerType.Player1);
            protoBattleStarts[PlayerType.Player2].NextCardStatId = Battle.BattleHelper.GetNexCardStatId(PlayerType.Player2);

            protoBattleStarts[PlayerType.Player1].EnemyPlayer.CardStatIds = GetAllCards(PlayerType.Player2, protoBattleStarts[PlayerType.Player2].HandCardStatIds);
            protoBattleStarts[PlayerType.Player2].EnemyPlayer.CardStatIds = GetAllCards(PlayerType.Player1, protoBattleStarts[PlayerType.Player1].HandCardStatIds);
        }
        
        private PlayerType GetEnemyPlayerType(PlayerType playerType)
        {
            return playerType == PlayerType.Player1 ? PlayerType.Player2 : PlayerType.Player1;
        }

    }
}
