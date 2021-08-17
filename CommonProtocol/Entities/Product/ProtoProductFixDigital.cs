using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoProductFixDigital
    {
        [Key(0)]
        public uint ProductId;

        [Key(1)]
        public StoreType StoreType;

        [Key(2)]
        public ProductType ProductType;

        [Key(3)]
        public string ProductText;

        [Key(4)]
        public MaterialType MaterialType;

        [Key(5)]
        public int Cost;
    }
}

