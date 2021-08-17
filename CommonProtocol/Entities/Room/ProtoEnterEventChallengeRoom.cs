using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEnterEventChallengeRoom : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public byte DeckNum;

        [Key(3)]
        public uint ChallengeEventId;

        [Key(4)]
        public int DifficultLevel;

        [Key(5)]
        public AkaEnum.Battle.BattleType BattleType;

        [Key(6)]
        public string BattleServerIp;

        [Key(7)]
        public bool IsStart;
    }
}
