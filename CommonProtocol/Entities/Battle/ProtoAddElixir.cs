using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoAddElixir : BaseProtocol
    {
        [Key(1)]
        public int AddElixir;
    }
}