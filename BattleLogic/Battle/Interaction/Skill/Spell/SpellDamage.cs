using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_DMG)]
    public class SpellDamage : ISpellSkill
    {
        private SkillEffectType[] _checkDamageBuffs = new SkillEffectType[]
        {
            SkillEffectType.BUFF_STATE_ACCUMULATE_SKILL_ATTACK,
            SkillEffectType.BUFF_STATE_TARGET
        };

        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            float damage = BuffStateCalculator.GetAtkWithConditions((int)option.Value2, target, SkillEffectTypes.SkillAttackByTargets, skill.AnimationData.TakeDamageTime);
            damage = BuffStateCalculator.GetAtkWithConditions(damage, target, SkillEffectTypes.SkillDefence, skill.AnimationData.TakeDamageTime);
            damage = BuffStateCalculator.GetAtkWithConditions(damage, target, _checkDamageBuffs, skill.AnimationData.TakeDamageTime);
            damage = CriticalDamageCalculator.GetCriticalDamageAsSkillAttack(performer, isCritical, (int)damage);
            var lastDamage = target.DecreaseHp(performer, damage, DamageReasonType.SkillAttack, skill.AnimationData.TakeDamageTime);

            return new ProtoSpellDamage()
            {
                DecreaseHpInfo = lastDamage,
                Hp = target.UnitData.UnitStatus.Hp,
                IsCritical = isCritical,
                Shields = target.UnitShields.GetShieldInfos(),
                SkillEffectType = SkillEffectType.COMMON_SPELL_DMG,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellDamage();
        }
    }
}