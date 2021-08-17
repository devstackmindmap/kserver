using AkaData;
using AkaEnum;
using System;

namespace BattleLogic
{
    public static class PassiveFactory
    {
        public static Passive CreatePassive(Unit unit, uint passiveConditionId)
        {
            var passive = Data.GetPassive(passiveConditionId);
            switch(passive.OperationConditionType)
            {
                case OperationConditionType.OverAndEqual:
                    return new PassiveOverAndEqual(unit, passive);
                case OperationConditionType.Equal:
                    return new PassiveEqual(unit, passive);
                case OperationConditionType.Over:
                    return new PassiveOver(unit, passive);
                case OperationConditionType.Less:
                    return new PassiveLess(unit, passive);
                case OperationConditionType.LessAndEqual:
                    return new PassiveLessAndEqual(unit, passive);
                default:
                    throw new Exception("Invalid OperationConditionType");
            }
        }
    }
}
