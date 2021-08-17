using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoStoreInfo
    {
        [Key(0)]
        public uint ProductId;

        [Key(1)]
        public ProductTableType ProductTableType;

        [Key(2)]
        public PlatformType PlatformType;

        [Key(3)]
        public string StoreProductId;

        [Key(4)]
        public string PurchaseToken;

        [Key(5)]
        public string TransactionId;
    }
}

