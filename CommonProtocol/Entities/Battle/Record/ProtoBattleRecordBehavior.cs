using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleRecordBehavior
    {
        [Key(0)]
        public PlayerType PlayerType;

        [Key(1)]
        public RecordBehaviorType BehaviorType;

        [Key(2)]
        public long Ticks;

        [Key(3)]
        public byte[] Behavior;

    }
}