using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserIdAndId : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint Id;
    }
}
