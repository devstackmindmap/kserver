using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanProfile : BaseProtocol
    {
        [Key(1)]
        public uint ClanId;

        [Key(2)]
        public ClanPublicType ClanPublicType;

        [Key(3)]
        public int JoinConditionRankPoint;

        [Key(4)]
        public string ClanName;

        [Key(5)]
        public uint ClanSymbolId;

        [Key(6)]
        public int RankPoint;

        [Key(7)]
        public int MemberCount;

        [Key(8)]
        public string ClanExplain;

        [Key(9)]
        public string CountryCode;

    }
}
