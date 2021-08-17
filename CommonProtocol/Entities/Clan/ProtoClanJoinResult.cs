using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanJoinResult : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public ProtoClanProfileAndMembers ClanInfo;
    }
}
