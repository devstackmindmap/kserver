using AkaEnum;

namespace AkaData
{
    public class DataPassiveCondition
    {
        public uint PassiveConditionId { get; set; }
        public uint CardStatId { get; set; }
        public PassiveType PassiveType { get; set; }
        public PassiveConditionType PassiveConditionType { get; set; }
        public OperationConditionType OperationConditionType { get; set; }
        public int PassiveConditionValue { get; set; }
        public int PassiveConditionLimitCount { get; set; }
    }
}
