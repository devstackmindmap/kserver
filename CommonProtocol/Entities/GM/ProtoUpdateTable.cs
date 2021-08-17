using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUpdateTable : BaseProtocol
    {
        [Key(1)]
        public string Command;
    }
}
