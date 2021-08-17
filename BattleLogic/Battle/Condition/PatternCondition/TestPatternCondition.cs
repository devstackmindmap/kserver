
using AkaData;

namespace BattleLogic
{

    
    public class TestPatternCondition : MonsterPatternCondition
    {
        public TestPatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
        }
        public override bool Check()
        {
            return false;
        }
    }
}
