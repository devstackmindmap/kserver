using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnRankPoint : BaseProtocol
    {
        [Key(1)]
        public int TeamRankPoint;

        [Key(2)]
        public int UserRankPoint;

        [Key(3)]
        public uint Wins;
    }
}
