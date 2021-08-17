
using AkaData;

namespace BattleLogic
{

    [PatternCondition(PatternType = AkaEnum.ActionPatternType.AllDeadUnitCount)]
    public class AllDeadUnitCountPatternCondition : SingleUsePatternCondition
    {
        protected System.Collections.Generic.HashSet<(AkaEnum.Battle.PlayerType, uint)> DeadUnits { get; private set; }

        public AllDeadUnitCountPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
            DeadUnits = new System.Collections.Generic.HashSet<(AkaEnum.Battle.PlayerType, uint)>();
        }
        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId)
        {
            if (Available() && DeadUnits.Add( (player, monsterId) ))
                CurrentActionPatternValue = DeadUnits.Count;
        }
    }
}
