using MessagePack;

namespace AkaRedisLogic
{
    [MessagePackObject]
    public class RedisValueRoomInfo
    {
        [Key(0)]
        public string RoomId;

        [Key(1)]
        public string BattleServerIp;

        [Key(2)]
        public string BattleServerPort;

        [Key(3)]
        public string SessionId;

        [Key(4)]
        public string Member;

        [Key(5)]
        public string MatchingGroupKey;

        [Key(6)]
        public string UserRankPoint;

        [Key(7)]
        public string RoomType;
        
        [Key(8)]
        public string Wins;

    }
}
