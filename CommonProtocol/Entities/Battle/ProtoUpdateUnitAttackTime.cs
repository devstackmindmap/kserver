using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol.Battle
{
    [MessagePackObject]
    public class ProtoUpdateUnitAttackTime : BaseProtocol
    {
        [Key(1)]
        public PlayerType PlayerType;
        
        [Key(2)]
        public uint UnitId;

        [Key(3)]
        public long NextAttackTime;

        [Key(4)]
        public float AttackSpeed;
    }
}