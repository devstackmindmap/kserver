using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]

    public class ProtoProductEventDigital
    {
        [Key(0)]
        public uint ProductId;

        [Key(1)]
        public long StartDateTime;

        [Key(2)]
        public long EndDateTime;

        [Key(3)]
        public StoreType StoreType;

        [Key(4)]
        public ProductType ProductType;

        [Key(5)]
        public string ProductText;

        [Key(6)]
        public MaterialType MaterialType;

        [Key(7)]
        public int SaleCost;

        [Key(8)]
        public int Cost;

        [Key(9)]
        public int CountOfPurchases;
    }
}

