
using AkaData;

namespace BattleLogic
{
    public class SingleUsePatternCondition : MonsterPatternCondition
    {

        private bool _canDoAction;
        public SingleUsePatternCondition(uint unitId, DataMonsterPatternCondition origin, AkaEnum.Battle.PlayerType player, AkaEnum.Battle.PlayerType enemyPlayer) : base(unitId, origin, player, enemyPlayer)
        {
            _canDoAction = true;
        }

        public override bool Check()
        {
            return Available() && base.Check();
        }

        public override void DidAction()
        {
            base.DidAction();
            _canDoAction = false;
        }
        
        public bool Available()
        {
            return _canDoAction;
        }
    }


}
