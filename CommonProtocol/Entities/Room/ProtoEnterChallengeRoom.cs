using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEnterChallengeRoom : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public byte DeckNum;

        [Key(3)]
        public uint Season;

        [Key(4)]
        public int Day;

        [Key(5)]
        public int DifficultLevel;

        [Key(6)]
        public AkaEnum.Battle.BattleType BattleType;

        [Key(7)]
        public string BattleServerIp;

        [Key(8)]
        public bool IsStart;
    }
}
