using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoFriendInfo : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public string Nickname;

        [Key(3)]
        public long RecentLoginDateTime;

        [Key(4)]
        public int CurrentSeason;

        [Key(5)]
        public int CurrentSeasonRankPoint;

        [Key(6)]
        public int NextSeasonRankPoint;

        [Key(7)]
        public uint ProfileIconId;

        [Key(8)]
        public string ClanName;

        [Key(9)]
        public ResultType ResultType;

        [Key(10)]
        public bool IsActivatedSquareObject;
    }
}
