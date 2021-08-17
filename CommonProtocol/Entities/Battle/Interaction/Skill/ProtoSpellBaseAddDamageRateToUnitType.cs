using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSpellBaseAddDamageRateToUnitType : BaseSkillProto
    {
        [Key(3)]
        public UnitType UnitType;

        [Key(4)]
        public float AddDamageRate;
    }
}