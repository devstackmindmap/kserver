using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_DMG_TARGET_HOLD_STATE_COUNT)]
    public sealed class SpellDamageTargetHoldStateCount : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var atk = performer.UnitData.UnitStatus.Atk;
            var attackerAtk = BuffStateCalculator.GetAtkWithConditions(atk, performer, SkillEffectTypes.SkillAttackByAttackers, skill.AnimationData.TakeDamageTime);
            var damage = BuffStateCalculator.GetAtkWithConditions(attackerAtk, target, SkillEffectTypes.SkillAttackByTargets, skill.AnimationData.TakeDamageTime);

            damage = CalculateValue(damage, target, performer, option.Value2, option.Value3);
            damage = BuffStateCalculator.GetAtkWithConditions(damage, target, SkillEffectTypes.SkillDefence, skill.AnimationData.TakeDamageTime);
            damage = CriticalDamageCalculator.GetCriticalDamageAsSkillAttack(performer, isCritical, (int)damage);
            var decreaseHpInfo = target.DecreaseHp(performer, damage, DamageReasonType.SkillAttack, skill.AnimationData.TakeDamageTime);

            return new ProtoSpellDamage()
            {
                DecreaseHpInfo = decreaseHpInfo,
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
            return new SpellDamageTargetHoldStateCount();
        }

        public float CalculateValue(float damage, Unit target, Unit performer, float value2, int value3)
        {
            var count = 0;
            switch ((TargetGroupType)value3)
            {
                case TargetGroupType.Ally:
                    count += GetBuffStateCount(performer);
                    break;
                case TargetGroupType.Enemy:
                    count += GetBuffStateCount(target);
                    break;
            }

            return damage + damage * count * value2;
        }

        private int GetBuffStateCount(Unit unit)
        {
            var count = 0;
            foreach (var buff in unit.UnitBuffs.Buffs)
            {
                if (buff.Value.IsValid() == false)
                    continue;

                count++;
            }

            return count;
        }
    }
}