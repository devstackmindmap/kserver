using System.Collections.Generic;

namespace AkaData
{
    public class DataChallengeEvent
    {
        public uint ChallengeEventNum { get; set; }
        public List<int> DifficultLevelList { get; set; }
        public List<uint> StageLevelIdList { get; set; }
        public List<uint> RewardIdList1 { get; set; }
        public List<uint> RewardIdList2 { get; set; }
        public List<uint> BanCardIdList { get; set; }
    }
}
