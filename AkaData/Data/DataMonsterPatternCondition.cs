using AkaEnum;

namespace AkaData
{
    public class DataMonsterPatternCondition
    {
        public uint MonsterPatternConditionId { get; set; }
        public ActionPatternType ActionPatternType { get; set; }
        public OperationConditionType OperationConditionType { get; set; }
        public int ActionPatternValue { get; set; }
    }
}
