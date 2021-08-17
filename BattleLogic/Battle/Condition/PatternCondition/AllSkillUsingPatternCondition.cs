
using AkaData;

namespace BattleLogic
{

    [PatternCondition(PatternType = AkaEnum.ActionPatternType.PerAllSkillUsingCount)]
    public class AllSkillUsingPatternCondition : MonsterPatternCondition
    {
        public AllSkillUsingPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId)
        {
            CurrentActionPatternValue++;
        }
    }
}
