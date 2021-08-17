using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserInfoChange : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public UserAdditionalInfoType UserInfoType;

        [Key(3)]
        public string UserValue;
    }
}
