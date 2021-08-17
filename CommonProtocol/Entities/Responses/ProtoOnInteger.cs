using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnInteger : BaseProtocol
    {
        [Key(1)]
        public int Value { get; set; }
    }
}
