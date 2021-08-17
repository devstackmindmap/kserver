using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMailDeleteAllResult : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ResultType> ProtoMailReadResults = new Dictionary<uint, ResultType>();
    }
}
