using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataReward
    {
        public uint RewardId { get; set; }
        public IList<uint> ItemIdList { get; set; }
        public int NeedEnergy { get; set; }
    }
}
