using System.Collections.Generic;

namespace AkaData
{
    public class DataUserLevel
    {
        public uint UserLevelId { get; set; }
        public ulong NeedExpForNextLevelUp { get; set; }
        public string Icon { get; set; }
        public uint RewardId { get; set; }
    }
}
