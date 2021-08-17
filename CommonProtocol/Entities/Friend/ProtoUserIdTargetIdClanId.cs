using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserIdTargetIdClanId : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint TargetId;

        [Key(3)]
        public uint ClanId;
    }
}
