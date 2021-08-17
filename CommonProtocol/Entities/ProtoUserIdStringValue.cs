using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserIdStringValue : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public string Value;
    }
}
