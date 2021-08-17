using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoTarget
    {
        [Key(0)]
        public PlayerType PlayerType;

        [Key(1)]
        public int UnitPositionIndex;
    }
}