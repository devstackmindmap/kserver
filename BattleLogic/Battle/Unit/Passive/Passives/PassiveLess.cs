using AkaData;

namespace BattleLogic
{
    public class PassiveLess : Passive
    {
        public PassiveLess(Unit unit, DataPassiveCondition passive) : base(unit, passive)
        {
        }

        public override bool IsConditionOk()
        {
            return _passiveConditionCount < _passive.PassiveConditionValue;
        }

        public override bool IsConditionOk(float baseValue, float compareValue)
        {
            return compareValue / baseValue * 100 < _passive.PassiveConditionValue;
        }

        public override bool IsConditionOk(float compareValue)
        {
            return compareValue < _passive.PassiveConditionValue;
        }
    }
}
