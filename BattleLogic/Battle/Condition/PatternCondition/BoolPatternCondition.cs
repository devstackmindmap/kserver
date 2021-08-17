
using AkaData;

namespace BattleLogic
{

    
    public class BoolPatternCondition : MonsterPatternCondition
    {
        private bool _checkValue;

        public BoolPatternCondition(bool checkValue, uint unitId, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, null, player, enemyPlayer)
        {
            _checkValue = checkValue;
        }
        public override bool Check()
        {
            return _checkValue;
        }
    }
}
