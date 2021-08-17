using AkaData;

namespace BattleLogic
{
    public class PassiveEqual : Passive
    {
        public PassiveEqual(Unit unit, DataPassiveCondition passive) : base(unit, passive)
        {
        }

        public override bool IsConditionOk()
        {
            return _passiveConditionCount == _passive.PassiveConditionValue;
        }

        public override bool IsConditionOk(float baseValue, float compareValue)
        {
            return compareValue / baseValue * 100 == _passive.PassiveConditionValue;
        }

        public override bool IsConditionOk(float compareValue)
        {
            return compareValue == _passive.PassiveConditionValue;
        }
    }
}
