using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCardInfoExceptCount : BaseProtocol
    {
        [Key(1)]
        public uint Id;

        [Key(2)]
        public uint Level;
    }
}
