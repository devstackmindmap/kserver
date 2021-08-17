using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_INCREASE_STATE_TIME)]
    public class SpellIncreaseStateTime : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var buffs = target.IncreaseConditionTime(option.Value2);

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
            return new SpellIncreaseStateTime();
        }
    }
}