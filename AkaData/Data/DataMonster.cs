using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataMonster
    {
        public uint MonsterId { get; set; }
        public uint BaseUnitId { get; set; }
        public List<uint> MonsterPatternIdList { get; set; }
        public MonsterType MonsterType { get; set; }
    }
}
