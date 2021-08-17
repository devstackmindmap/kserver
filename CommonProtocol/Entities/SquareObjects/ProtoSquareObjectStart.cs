
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSquareObjectStart : BaseProtocol
    {
        [Key(1)]
        public uint UserId { get; set; }

        [Key(2)]
        public uint ObjectLevel { get; set; }

    }
}
