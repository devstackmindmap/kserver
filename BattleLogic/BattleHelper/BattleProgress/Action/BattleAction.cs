using AkaEnum;

namespace BattleLogic
{
    public abstract class BattleAction
    {
        public Unit Attacker;

        public BattleAction(Unit attacker)
        {
            Attacker = attacker;
        }

        public abstract BattleActionResult DoAction();
    }
}
