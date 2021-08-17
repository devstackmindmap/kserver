
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSquareObjectLevelUp : BaseProtocol
    {
        [Key(1)]
        public uint UserId { get; set; }

        [Key(2)]
        public int UsedExp { get; set; }

    }
}
