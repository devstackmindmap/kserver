using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanOutResult : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public ProtoClanRecommend ClanInfos;
    }
}
