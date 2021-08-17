using AkaEnum;
using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetReward : BaseProtocol
    {
        [Key(1)]
        public List<ProtoItemResult> ItemResults;

        [Key(2)]
        public RewardResultType Result;

    }
}
