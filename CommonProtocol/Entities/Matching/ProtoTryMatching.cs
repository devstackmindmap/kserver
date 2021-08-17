using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoTryMatching : BaseProtocol
    {
        [Key(1)]
        public byte DeckNum;

        [Key(2)]
        public AkaEnum.Battle.BattleType BattleType;

        [Key(3)]
        public uint UserId;

        [Key(4)]
        public string BattleServerIp;

        [Key(5)]
        public int BattleServerPort;

        [Key(6)]
        public int GroupCode;

    }
}
