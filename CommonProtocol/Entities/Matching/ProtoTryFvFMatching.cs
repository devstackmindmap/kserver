using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoTryFvFMatching : ProtoTryMatching
    {
        [Key(8)]
        public uint FriendUserId;
    }
}
