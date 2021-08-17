using AkaEnum;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEnqueuedAction : BaseProtocol
    {
        [Key(1)]
        public PlayerType PlayerType;

        [Key(2)]
        public uint UnitId;

        [Key(3)]
        public BattleInteractionType BattleInteractionType;
    }
}