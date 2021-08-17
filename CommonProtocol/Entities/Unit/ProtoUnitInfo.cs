using MessagePack;

namespace CommonProtocol.Login
{
    [MessagePackObject]
    public class ProtoUnitInfo : BaseProtocol
    {
        [Key(1)]
        public uint Id;

        [Key(2)]
        public uint Level;

        [Key(3)]
        public int Count;

        [Key(4)]
        public uint MaxRankLevel;

        [Key(5)]
        public int CurrentSeasonRankPoint;

        [Key(6)]
        public int NextSeasonRankPoint;

        [Key(7)]
        public uint SkinId;

        [Key(8)]
        public uint MaxVirtualRankLevel;

        [Key(9)]
        public int VirtualRankPoint;

        [Key(10)]
        public uint VirtualCurrentRankLevel;
    }
}
