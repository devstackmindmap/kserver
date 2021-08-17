using AkaEnum;

namespace AkaData
{
    public class DataSquareObjectPlanetAgency
    {
        public uint PlanetAgencyId { get; set; }
        public uint PlanetAgencyLevel { get; set; }
        public double PlanetCoreEnergyAdditionalRate { get; set; }
        public int NeedExpForNextLevelUp { get; set; }
        public int NeedGoldForNextLevelUp { get; set; }
    }
}