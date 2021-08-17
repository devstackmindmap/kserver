using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattlePlayingInfo
    {
        [Key(0)]
        public string RoomId;

        [Key(1)]
        public string BattleServerIp;

        [Key(2)]
        public int BattleServerPort;
    }
}
