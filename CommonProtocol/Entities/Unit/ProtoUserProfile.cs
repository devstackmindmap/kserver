using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserProfile : BaseProtocol
    {
        [Key(1)]
        public int CurrentSeason;

        [Key(2)]
        public uint MaxRankLevel;

        [Key(3)]
        public int CurrentSeasonRankPoint;

        [Key(4)]
        public int NextSeasonRankPoint;

        [Key(5)]
        public int MaxRankPoint;

        [Key(6)]
        public int RankVictoryCount;

        [Key(7)]
        public uint ProfileIconId;

        [Key(8)]
        public ProtoOnGetRecentDeck ProtoOnGetRecentDeck;

        [Key(9)]
        public uint Level;

        [Key(10)]
        public ulong Exp;

        [Key(11)]
        public uint ClanId;

        [Key(12)]
        public ClanMemberGrade ClanMemberGrade;

        [Key(13)]
        public string ClanName;

        [Key(14)]
        public uint ClanSymbolId;

        [Key(15)]
        public int MaxVirtualRankPoint = 0;

        [Key(16)]
        public int ChallengeClearCount = 0;
    }
}
