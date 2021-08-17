using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoAddInvite : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public string InviteCode;
    }
}
