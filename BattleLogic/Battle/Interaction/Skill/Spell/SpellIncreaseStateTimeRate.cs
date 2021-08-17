using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_INCREASE_STATE_TIME_RATE)]
    public class SpellIncreaseStateTimeRate : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var buffs = target.IncreaseConditionTimeRate(option.Value2);

            return new ProtoSpellIncreaseConditionTime()
            {
                SkillEffectType = SkillEffectType.COMMON_SPELL_INCREASE_STATE_TIME,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                Buffs = buffs
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellIncreaseStateTimeRate();
        }
    }
}