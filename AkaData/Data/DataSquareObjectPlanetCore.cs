using AkaEnum;

namespace AkaData
{
    public class DataSquareObjectPlanetCore
    {
        public uint PlanetCoreId { get; set; }
        public uint PlanetCoreLevel { get; set; }
        public int MaxPlanetCoreEnergy { get; set; }
        public int StartEnergy { get; set; }
        public int NeedExpForNextLevelUp { get; set; }
        public int NeedGoldForNextLevelUp { get; set; }
    }
}