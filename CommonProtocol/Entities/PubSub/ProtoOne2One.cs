using MessagePack;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoOne2One : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
    }
}
