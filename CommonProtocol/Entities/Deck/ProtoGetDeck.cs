using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetDeck : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public ModeType ModeType;
    }
}
