using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoInviteCode : BaseProtocol
    {
        [Key(1)]
        public string InviteCode;

        [Key(2)]
        public string CountryCode;
    }
}
