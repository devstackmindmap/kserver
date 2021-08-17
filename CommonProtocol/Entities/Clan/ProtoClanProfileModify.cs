using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanProfileModify : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint ClanId;

        [Key(3)]
        public uint ClanSymbolId;

        [Key(4)]
        public ClanPublicType ClanPublicType;

        [Key(5)]
        public int JoinConditionRankPoint;

        [Key(6)]
        public string CountryCode;

        [Key(7)]
        public string ClanExplain;
    }
}
