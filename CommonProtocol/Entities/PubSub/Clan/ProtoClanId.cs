using MessagePack;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoClanId : BaseProtocol
    {
        [Key(1)]
        public uint ClanId;
    }
}
