using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanInfo
    {
        [Key(0)]
        public uint ClanId;

        [Key(1)]
        public string CountryCode;

        [Key(2)]
        public string ClanName;

        [Key(3)]
        public uint ClanSymbolId;

        [Key(4)]
        public int RankPoint;

        [Key(5)]
        public int MemberCount;

        [Key(6)]
        public uint ClanMasterUserId;

        [Key(7)]
        public string InviteCode;
    }
}
