using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRequestFriend : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint FriendId;
    }
}
