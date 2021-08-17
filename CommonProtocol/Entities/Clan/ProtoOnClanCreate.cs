using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnClanCreate : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public ProtoClanProfileAndMembers ClanInfo;
    }
}
