using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBuySeasonPass : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public string StoreProductId;

        [Key(3)]
        public ProtoInfusionBox InfusionBox;
    }
}

