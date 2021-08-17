using System.Collections.Generic;

namespace AkaData
{
    public class DataMonsterPatternFlow
    {
        public uint MonsterPatternFlowId { get; set; }
        public List<uint> MonsterPatternConditionIdList { get; set; }
        public uint TransMonsterPatternId { get; set; }
    }
}
