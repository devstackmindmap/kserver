using AkaEnum;
using MessagePack;
using PlayerType = AkaEnum.Battle.PlayerType;

namespace CommonProtocol
{
    [Union(0, typeof(ProtoSpellDamage))]
    [Union(1, typeof(ProtoSpellShield))]
    [Union(2, typeof(ProtoSpellDisapearance))]
    [Union(3, typeof(ProtoSpellIncreaseConditionTime))]
    [Union(4, typeof(ProtoCommonBuffState))]
    [Union(5, typeof(ProtoBuffFail))]
    [Union(6, typeof(ProtoSkillEmpty))]
    [Union(7, typeof(ProtoSpellBaseAddDamageRateToUnitType))]
    [Union(8, typeof(ProtoIncreaseAtk))]
    [Union(9, typeof(ProtoSpellBuffStackCount))]
    [Union(10, typeof(ProtoSetCurrentHp))]
    [Union(11, typeof(ProtoIncreaseAggro))]
    [Union(12, typeof(ProtoGrowthMaxHp))]
    [Union(13, typeof(ProtoHandCardCostDecrease))]
    public abstract class BaseSkillProto
    {
        [Key(0)]
        public SkillEffectType SkillEffectType;

        [Key(1)]
        public PlayerType TargetPlayerType;

        [Key(2)]
        public uint TargetUnitId;
    }
}