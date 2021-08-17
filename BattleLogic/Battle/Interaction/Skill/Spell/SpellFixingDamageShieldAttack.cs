using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_FIXING_DMG_SHIELD_ATTACK)]
    public class SpellFixingDamageShieldAttack : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var damage = performer.Shield * option.Value2;
            var decreaseHpInfo = target.DecreaseHp(damage, DamageReasonType.SkillShieldAttack, skill.AnimationData.TakeDamageTime, false);
            performer.ShieldEnd();

            return new ProtoSpellDamage()
            {
                SkillEffectType = SkillEffectType.COMMON_SPELL_DMG,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                DecreaseHpInfo = decreaseHpInfo,
                Hp = target.UnitData.UnitStatus.Hp,
                IsCritical = false,
                Shields = target.UnitShields.GetShieldInfos()
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellFixingDamageShieldAttack();
        }
    }
}