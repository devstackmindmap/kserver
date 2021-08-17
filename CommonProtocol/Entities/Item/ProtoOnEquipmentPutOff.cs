using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnEquipmentPutOff : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;
    }
}

