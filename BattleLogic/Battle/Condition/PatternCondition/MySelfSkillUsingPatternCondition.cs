
using AkaData;
namespace BattleLogic
{

    [PatternCondition(PatternType = AkaEnum.ActionPatternType.PerMyselfSkillUsingCount)]
    public class MySelfSkillUsingPatternCondition : MonsterPatternCondition
    {
        public MySelfSkillUsingPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override void UpdateCondition(AkaEnum.Battle.PlayerType player, uint monsterId)
        {
            if ( IsMe(player, monsterId) )
            {
                CurrentActionPatternValue++;
            }
        }
    }
}
