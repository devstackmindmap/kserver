using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserIdByteValue : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public byte Value;
    }
}
