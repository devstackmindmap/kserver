using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoProductEventReal
    {
        [Key(0)]
        public uint ProductId;

        [Key(1)]
        public long StartDateTime;

        [Key(2)]
        public long EndDateTime;

        [Key(3)]
        public string StoreProductId;

        [Key(4)]
        public StoreType StoreType;
        
        [Key(5)]
        public ProductType ProductType;

        [Key(6)]
        public string ProductText;

        [Key(7)]
        public int SaleCost;

        [Key(8)]
        public int Cost;

        [Key(9)]
        public int CountOfPurchases;
    }
}

