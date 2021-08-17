using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetReward : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public Dictionary<uint, uint> ClassIdWithItemValueList;
        
        [Key(3)]
        public GettingRewardType GettingRewardType;

    }
}
