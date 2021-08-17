using System.Collections.Generic;

namespace AkaData
{
    public class DataMonsterPattern
    {
        public uint MonsterPatternId { get; set; }
        public List<uint> ActivePatternConditionIdList { get; set; }
        public List<uint> DeactivePatternConditionIdList { get; set; }
        public List<uint> MonsterPatternConditionIdList { get; set; }
        public uint CardStatId { get; set; }
        public int RepeatCount { get; set; }
        public List<uint> MonsterPatternFlowIdList { get; set; }
    }
}
