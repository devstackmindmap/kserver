using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnUnitProfile : BaseProtocol
    {
        [Key(1)]
        public List<ProtoUnitProfile> UnitProfiles = new List<ProtoUnitProfile>();

        [Key(2)]
        public uint CurrentSeason;
    }


    [MessagePackObject]
    public class ProtoUnitProfile
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
        public uint MaxVirtualRankLevel;

        [Key(7)]
        public uint CurrentVirtualRankLevel;

        [Key(8)]
        public int CurrentVirtualRankPoint;
    }
}
