using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoLogin : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public List<uint> FriendIds = new List<uint>();

        [Key(3)]
        public uint ClanId;

        [Key(4)]
        public List<uint> ClanMemberIds = new List<uint>();

        [Key(5)]
        public MessageType StateMessageType;
    }
}
