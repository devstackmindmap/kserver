using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBuyProduct : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint ProductId;

        [Key(3)]
        public ProductTableType ProductTableType;
    }
}

