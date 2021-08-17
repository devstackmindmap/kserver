using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_DMG_AS_MINE_ATK)]
    public class SpellDamageAsMineAtk : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var atk = performer.UnitData.UnitStatus.Atk;
            var attackerAtk = BuffStateCalculator.GetAtkWithConditions(atk, performer, SkillEffectTypes.SkillAttackByAttackers, skill.AnimationData.TakeDamageTime);
            var damage = BuffStateCalculator.GetAtkWithConditions(attackerAtk, target, SkillEffectTypes.SkillAttackByTargets, skill.AnimationData.TakeDamageTime);

            damage = BuffStateCalculator.GetAtkWithConditions(damage, target, SkillEffectTypes.SkillDefence, skill.AnimationData.TakeDamageTime);
            damage = CriticalDamageCalculator.GetCriticalDamageAsSkillAttack(performer, isCritical, (int)(damage * option.Value2));
            var decreaseHpInfo = target.DecreaseHp(performer, damage, DamageReasonType.SkillAsMineAttack, skill.AnimationData.TakeDamageTime);

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
            return new SpellDamageAsMineAtk();
        }
    }
}
