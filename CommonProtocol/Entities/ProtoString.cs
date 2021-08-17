using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoString : BaseProtocol
    {
        [Key(1)]
        public string StringValue;
    }
}
