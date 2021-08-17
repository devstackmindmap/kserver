using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoLevelUp : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public PieceType PieceType;

        [Key(3)]
        public uint ClassId;
    }
}
