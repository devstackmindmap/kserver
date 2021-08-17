using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_FIXING_DMG_POISON_BOMB)]
    public class SpellFixingDamagePoisonBomb : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var poisonBuff = target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_POISON);
            var decreaseHpInfo = new ProtoTargetDecreaseHpInfo();

            if (poisonBuff != null)
            {
                var stack = target.GetPoisonStack();
                if (target.IsPoison == false)
                {
                    target.EnqueueBuffEnd(poisonBuff);
                }
                else
                {
                    var damage = stack * option.Value2;
                    var buffAccumulateSkillAttack = target.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_ACCUMULATE_SKILL_ATTACK);
                    buffAccumulateSkillAttack?.CalculateValue(ref damage, null);
                    decreaseHpInfo = target.DecreaseHp(performer, damage, DamageReasonType.SkillAttack, skill.AnimationData.TakeDamageTime);
                    target.EnqueueBuffEnd(poisonBuff);
                }
            }

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
            return new SpellFixingDamagePoisonBomb();
        }
    }
}