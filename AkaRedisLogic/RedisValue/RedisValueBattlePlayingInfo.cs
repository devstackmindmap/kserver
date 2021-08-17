using MessagePack;

namespace AkaRedisLogic
{
    [MessagePackObject]
    public class RedisValueBattlePlayingInfo
    {
        [Key(0)]
        public string RoomId;

        [Key(1)]
        public string BattleServerIp;

        [Key(2)]
        public long BattleStartDate;

        [Key(3)]
        public string BattleServerPort;
    }
}
