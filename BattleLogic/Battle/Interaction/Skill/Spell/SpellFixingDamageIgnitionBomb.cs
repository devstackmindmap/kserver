using AkaData;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_FIXING_DMG_IGNITION_BOMB)]
    public class SpellFixingDamageIgnitionBomb : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var ignitionBuff = target.GetConditionBuffSkill(SkillEffectType.BUFF_STATE_IGNITION);
            var decreaseHpInfo = new ProtoTargetDecreaseHpInfo();

            if (ignitionBuff != null)
            {
                if (ignitionBuff.IsValid() == false || ignitionBuff.Value <= 0)
                {
                    target.EnqueueBuffEnd(ignitionBuff);
                }
                else
                {
                    var atk = BuffStateCalculator.GetAtkWithConditions(performer.UnitData.UnitStatus.Atk, performer, SkillEffectTypes.SkillAttackByAttackers, skill.AnimationData.TakeDamageTime);
                    float damage = ignitionBuff.Value * option.Value2 * atk;
                    decreaseHpInfo = target.DecreaseHp(performer, damage, DamageReasonType.SkillAttack, skill.AnimationData.TakeDamageTime);
                    target.EnqueueBuffEnd(ignitionBuff);
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
            return new SpellFixingDamageIgnitionBomb();
        }
    }
}