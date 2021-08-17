using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanProfileAndMembers : BaseProtocol
    {
        [Key(1)]
        public ProtoClanProfile ClanProfile;

        [Key(2)]
        public List<ProtoClanMember> ClanMembers;
    }
}
