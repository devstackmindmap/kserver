using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRecommendClanInfo
    {
        [Key(0)]
        public uint ClanId;

        [Key(1)]
        public string ClanName;

        [Key(2)]
        public uint ClanSymbolId;

        [Key(3)]
        public int RankPoint;

        [Key(4)]
        public int MemberCount;
    }
}
