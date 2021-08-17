using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUnitInfoForRecentDeck
    {
        [Key(0)]
        public uint UnitId;

        [Key(1)]
        public uint Level;

        [Key(2)]
        public uint MaxRankLevel;

        [Key(3)]
        public int CurrentSeasonRankPoint;

        [Key(4)]
        public int NextSeasonRankPoint;

        [Key(5)]
        public uint SkinId;

        [Key(6)]
        public ProtoWeaponInfoForBattle WeaponInfo;

        [Key(7)]
        public uint CurrentVirtualRankLevel;

        [Key(8)]
        public uint MaxVirtualRankLevel;

        [Key(9)]
        public int CurrentVirtualRankPoint;
    }
}