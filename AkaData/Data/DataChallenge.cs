using System.Collections.Generic;

namespace AkaData
{
    public class DataChallenge
    {
        public uint Season { get; set; }
        public int Day { get; set; }
        public List<int> DifficultLevelList { get; set; }
        public List<uint> StageLevelIdList { get; set; }
        public List<uint> RewardIdList1 { get; set; }
        public List<uint> RewardIdList2 { get; set; }
        public List<uint> BanCardIdList { get; set; }


    }
}
