using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEventGetReward : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public List<ProtoItemResult> ItemResults;
    }
}
