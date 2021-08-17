using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetProducts : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public PlatformType PlatformType;

        [Key(3)]
        public string LanguageType;
    }
}

