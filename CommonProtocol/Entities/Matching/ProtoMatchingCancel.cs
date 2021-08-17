using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMatchingCancel : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
    }
}
