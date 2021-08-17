using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataRankTierMatching
    {
        public int RankTierMatchingId { get; set; }
        public int TeamRankPointForMatching { get; set; }
        public List<uint> StageRoundIdList { get; set; }
        public MatchingPriorityType Priority { get; set; }
        public int AiMatchingWaitingMillisecond { get; set; }
    }
}
