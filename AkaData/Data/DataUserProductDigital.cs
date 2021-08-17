using AkaEnum;

namespace AkaData
{
    public class DataUserProductDigital
    {
        public uint ProductId { get; set; }
        public int SaleDurationHour { get; set; }
        public StoreType StoreType { get; set; }
        public ProductType ProductType { get; set; }
        public MaterialType MaterialType { get; set; }
        public int SaleCost { get; set; }
        public int Cost { get; set; }
        public int CountOfPurchases { get; set; }
    }
}