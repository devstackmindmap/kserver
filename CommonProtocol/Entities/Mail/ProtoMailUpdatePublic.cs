using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMailUpdatePublic : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public string LanguageType;
    }
}
