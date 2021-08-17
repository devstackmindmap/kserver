using System;
using System.Collections.Generic;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_FIXING_DMG_ATTACK_USE_THE_TARGET_SHIELD)]
    public sealed class SpellFixingDamageAttackUseTheTargetShield : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var damage = target.Shield;
            target.ShieldEnd();
            var decreaseHpInfo = target.DecreaseHp(damage, DamageReasonType.SkillShieldAttack, skill.AnimationData.TakeDamageTime, false);

            return new ProtoSpellDamage()
            {
                SkillEffectType = SkillEffectType.COMMON_SPELL_DMG,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                DecreaseHpInfo = decreaseHpInfo,
                Hp = target.UnitData.UnitStatus.Hp,
                IsCritical = false,
                Shields = new List<ProtoShieldInfo>()
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellFixingDamageAttackUseTheTargetShield();
        }
    }
}