using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoLogin : BaseProtocol
    {
        [Key(1)]
        public string SocialAccount;

        [Key(2)]
        public PlatformType PlatformType;

        [Key(3)]
        public string LanguageType;
    }
}
