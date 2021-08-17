using MessagePack;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoOne2N : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint FriendId;
    }
}
