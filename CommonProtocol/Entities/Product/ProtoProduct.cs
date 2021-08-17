using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoProduct
    {
        [Key(0)]
        public List<ProtoReward> Rewards;
    }
}

