using AkaEnum;

namespace AkaData
{
    public class DataSquareObjectPlanetBox
    {
        public uint PlanetBoxId { get; set; }
        public uint SquareObjectLevel { get; set; }
        public uint PlanetBoxLevel { get; set; }
        public uint RewardId { get; set; }
        public uint NeedExpForNextLevelUp { get; set; }
        public int GiveToSquareObjectExp { get; set; }
        public uint DestroyRewardId { get; set; }
        public int GiveToLoseSquareObjectExp { get; set; }
    }
}