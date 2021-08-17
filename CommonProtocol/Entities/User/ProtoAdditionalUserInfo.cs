using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoAdditionalUserInfo : BaseProtocol
    {
        [Key(1)]
        public bool IsAlreadyFreeNicknameChange;

        [Key(2)]
        public long RecentDateTimeCountryChange;

        [Key(3)]
        public sbyte DailyRankVictoryGoldRewardCount;

        [Key(4)]
        public long DailyRankVictoryGoldRewardDateTime;

        [Key(5)]
        public int RewardedRankSeason;

        [Key(6)]
        public List<ContentsType> UnlockContents;

        [Key(7)]
        public uint MaxVirtualRankLevel;

        [Key(8)]
        public int CurrentVirtualRankPoint;

        [Key(9)]
        public int MaxVirtualRankPoint;

        [Key(10)]
        public List<uint> EnablePassList;

        [Key(11)]
        public int DailyQuestRefreshCount;

        [Key(12)]
        public int DailyQuestAddCount;

        [Key(13)]
        public int AddDeck;
    }
}
