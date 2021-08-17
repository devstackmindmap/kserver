using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetBattleServer : BaseProtocol
    {
        [Key(1)]
        public string BattleServerIp;

        [Key(2)]
        public int BattleServerPort;

        [Key(3)]
        public string MatchingServerIp;

        [Key(4)]
        public int MatchingServerPort;
    }
}
