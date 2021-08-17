using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoInfusionBoxOpen : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public InfusionBoxType Type;
    }
}
