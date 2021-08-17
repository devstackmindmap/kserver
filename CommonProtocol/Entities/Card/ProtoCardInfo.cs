using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCardInfo : BaseProtocol
    {
        [Key(1)]
        public uint Id;

        [Key(2)]
        public uint Level;

        [Key(3)]
        public int Count;
    }
}
