using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRankPoint : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public byte DeckNum;

        [Key(3)]
        public AkaEnum.ModeType DeckModeType;

        [Key(4)]
        public AkaEnum.RankType RankType;
    }
}
