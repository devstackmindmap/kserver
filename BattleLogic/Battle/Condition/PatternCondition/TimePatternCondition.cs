
using AkaData;

namespace BattleLogic
{

    [PatternCondition(PatternType = AkaEnum.ActionPatternType.PerTimeSecond)]
    public class TimePatternCondition : MonsterPatternCondition
    {
        public TimePatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override void UpdateCondition()
        {
            CurrentActionPatternValue++;
        }
    }
}
