using MessagePack;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoWeb2One : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
    }
}
