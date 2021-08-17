using AkaEnum;

namespace AkaData
{
    public class DataUserProductReal
    {
        public uint ProductId { get; set; }
        public string AosStoreProductId { get; set; }
        public string IosStoreProductId { get; set; }
        public int SaleDurationHour { get; set; }
        public StoreType StoreType { get; set; }
        public ProductType ProductType { get; set; }
        public int SaleCost { get; set; }
        public int Cost { get; set; }
        public int CountOfPurchases { get; set; }
    }
}