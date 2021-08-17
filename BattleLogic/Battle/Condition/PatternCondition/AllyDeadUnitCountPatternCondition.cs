
using AkaData;

namespace BattleLogic
{

    [PatternCondition(PatternType = AkaEnum.ActionPatternType.AllyDeadUnitCount)]
    public class AllyDeadUnitCountPatternCondition : AllDeadUnitCountPatternCondition
    {

        public AllyDeadUnitCountPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId)
        {
            if (Available() && player == Player &&  DeadUnits.Add( (player, monsterId) ))
                CurrentActionPatternValue = DeadUnits.Count;
        }
    }
}
