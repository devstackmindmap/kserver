using System.Collections.Generic;
using AkaEnum;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoTargetUnitInfo
    {
        [Key(0)]
        public PlayerType PlayerType;

        [Key(1)]
        public uint UnitId;

        [Key(2)]
        public int Hp;

        [Key(3)]
        public ProtoTargetDecreaseHpInfo DecreaseHpInfo;

        [Key(4)]
        public bool IsCritical;

        [Key(5)]
        public List<ProtoShieldInfo> Shields;

        [Key(6)]
        public bool IsShieldIgnore;

        [Key(7)]
        public List<SkillEffectType> RemoveBuffs;
    }
}