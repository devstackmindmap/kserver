using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoReward
    {
        [Key(0)]
        public uint RewardId;

        [Key(1)]
        public RewardType RewardType;

        [Key(2)]
        public List<List<ProtoItem>> Items;
    }
}

