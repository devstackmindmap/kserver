using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserIdTargetId : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint TargetId;
    }
}
