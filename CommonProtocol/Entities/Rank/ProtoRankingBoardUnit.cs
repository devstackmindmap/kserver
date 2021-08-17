using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRankingBoardUnit : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint UnitId;

        [Key(3)]
        public string CountryCode;
    }
}
