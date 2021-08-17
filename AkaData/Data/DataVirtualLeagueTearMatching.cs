
using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataVirtualLeagueTearMatching
    {
        public int VirtualLeagueMatchingId { get; set; }
        public int TeamRankPointForMatching { get; set; }
        public List<uint> StageRoundIdList { get; set; }
    }
}
