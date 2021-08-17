using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_FIXING_DMG_CUR_HP_RATE_ATTACK)]
    public class SpellFixingDamageCurHpRateAttack : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var damage = (int)(target.UnitData.UnitStatus.Hp * option.Value2);
            var decreaseHpInfo = target.DecreaseHp(damage, DamageReasonType.SkillCurHpRateAttack, skill.AnimationData.TakeDamageTime, false);

            return new ProtoSpellDamage()
            {
                DecreaseHpInfo = decreaseHpInfo,
                Hp = target.UnitData.UnitStatus.Hp,
                IsCritical = false,
                Shields = target.UnitShields.GetShieldInfos(),
                SkillEffectType = SkillEffectType.COMMON_SPELL_DMG,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellFixingDamageCurHpRateAttack();
        }
    }
}