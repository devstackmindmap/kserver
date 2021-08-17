using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserId : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
    }
}
