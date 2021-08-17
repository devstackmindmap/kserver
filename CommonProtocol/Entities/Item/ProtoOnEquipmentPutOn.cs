using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnEquipmentPutOn : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;
    }
}
