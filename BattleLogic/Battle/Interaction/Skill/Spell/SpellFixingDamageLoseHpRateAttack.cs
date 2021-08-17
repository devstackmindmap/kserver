using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_FIXING_DMG_LOSE_HP_RATE_ATTACK)]
    public class SpellFixingDamageLoseHpRateAttack : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var loseHp = GetLoseHp((TargetGroupType)option.Value3, performer, target);
            var damage = (int)(loseHp * option.Value2);
            var decreaseHpInfo = target.DecreaseHp(damage, DamageReasonType.SkillLoseHpRateAttack, skill.AnimationData.TakeDamageTime, false);

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
            return new SpellFixingDamageLoseHpRateAttack();
        }

        private int GetLoseHp(TargetGroupType groupType, Unit performer, Unit target)
        {
            switch (groupType)
            {
                case TargetGroupType.Ally:
                    return performer.UnitData.UnitStatus.MaxHp - performer.UnitData.UnitStatus.Hp;
                case TargetGroupType.Enemy:
                    return target.UnitData.UnitStatus.MaxHp - target.UnitData.UnitStatus.Hp;
            }

            return 0;
        }
    }
}