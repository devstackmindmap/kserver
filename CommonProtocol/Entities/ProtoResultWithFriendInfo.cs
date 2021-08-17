using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoResultWithFriendInfo : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public ProtoFriendInfo FriendInfo;
    }
}
