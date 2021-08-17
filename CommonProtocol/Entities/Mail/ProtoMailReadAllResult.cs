using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMailReadAllResult : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoMailActionResult> ProtoMailReadResults = new Dictionary<uint, ProtoMailActionResult>();
    }
}
