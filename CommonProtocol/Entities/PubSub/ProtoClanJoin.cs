using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoClanJoin : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint ClanId;

        [Key(3)]
        public List<uint> ClanMemberIds = new List<uint>();
    }
}
