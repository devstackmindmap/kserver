using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoResult : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;
    }
}
