using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRankData
    {
        [Key(0)]
        public int CurrentSeasonRankPoint;

        [Key(1)]
        public uint MaxRankLevel;

        [Key(2)]
        public uint CurrentRankLevel;

        [Key(3)]
        public int NextSeasonRankPoint;

        [Key(4)]
        public int MaxRankPoint;

        [Key(5)]
        public uint Wins;
    }
}