using System.Collections.Generic;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSpellDamage : BaseSkillProto
    {
        [Key(3)]
        public ProtoTargetDecreaseHpInfo DecreaseHpInfo;

        [Key(4)]
        public int Hp;

        [Key(5)]
        public bool IsCritical;

        [Key(6)]
        public List<ProtoShieldInfo> Shields;

        [Key(7)]
        public bool IsShieldIgnore;
    }
}