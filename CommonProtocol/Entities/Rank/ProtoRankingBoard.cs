using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRankingBoard : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public string CountryCode;
    }
}
