using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoReEnterRoom : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
    }
}
