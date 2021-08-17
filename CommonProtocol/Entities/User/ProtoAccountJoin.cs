using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoAccountJoin : BaseProtocol
    {
        [Key(1)]
        public string SocialAccount;

        [Key(2)]
        public string NickName;

        [Key(3)]
        public string PushKey;

        [Key(4)]
        public byte PushAgree;

        [Key(5)]
        public byte NightPushAgree;

        [Key(6)]
        public PlatformType PlatformType;

        [Key(7)]
        public string LanguageType;

        [Key(8)]
        public byte TermsAgree;
    }
}
