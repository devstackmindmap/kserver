using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoProductFixReal
    {
        [Key(0)]
        public uint ProductId;

        [Key(1)]
        public string StoreProductId;

        [Key(2)]
        public StoreType StoreType;

        [Key(3)]
        public ProductType ProductType;

        [Key(4)]
        public string ProductText;

        [Key(5)]
        public int Cost;
    }
}

