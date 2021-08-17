using AkaEnum.Battle;
using CommonProtocol;
using System.Collections.Generic;

namespace BattleLogic
{
    public sealed class AiDataFillManager : DataFillManager
    {
        protected override void FillCardInfo(Dictionary<PlayerType, ProtoBeforeBattleStart> protoBattleStarts)
        {
            protoBattleStarts[PlayerType.Player1].HandCardStatIds = GetHandCardStatIds(PlayerType.Player1);

            protoBattleStarts[PlayerType.Player1].NextCardStatId = Battle.BattleHelper.GetNexCardStatId(PlayerType.Player1);

            protoBattleStarts[PlayerType.Player1].EnemyPlayer.CardStatIds = Battle.BattleHelper.BattlePatternBehavior.GetPatternOfCardStatIdList();
            protoBattleStarts[PlayerType.Player2].EnemyPlayer.CardStatIds = GetAllCards(PlayerType.Player1, protoBattleStarts[PlayerType.Player1].HandCardStatIds);
        }

    }
}
