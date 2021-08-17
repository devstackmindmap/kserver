using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBuyProductDigital : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint ProductId;

        [Key(3)]
        public ProductTableType ProductTableType;

        [Key(4)]
        public uint ItemValue;
    }
}

