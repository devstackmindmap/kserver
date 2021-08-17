using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnInfusionBoxOpen : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public ProtoInfusionBoxOpenInfo OpenInfo;
    }
}
