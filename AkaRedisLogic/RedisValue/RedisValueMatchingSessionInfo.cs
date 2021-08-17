using MessagePack;

namespace AkaRedisLogic
{
    [MessagePackObject]
    public class RedisValueMatchingSessionInfo
    {
        [Key(0)]
        public string Member;

        [Key(1)]
        public string RoomId;

        [Key(2)]
        public string MatchingGroupKey;
    }
}
