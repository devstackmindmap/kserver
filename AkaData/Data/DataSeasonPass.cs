using AkaEnum;

namespace AkaData
{
    public class DataSeasonPass
    {
        public uint SeasonPassId { get; set; }
        public SeasonPassType SeasonPassType { get; set; }
        public uint Season { get; set; }
        public uint QuestGroupId { get; set; }
        public MaterialType MaterialType { get; set; }
        public int Value { get; set; }

        public uint ProductId { get; set; }

        public string AosStoreProductId { get; set; }
        public string IosStoreProductId { get; set; }
    }
}
