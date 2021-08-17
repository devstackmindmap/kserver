using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_CRITICAL_DMG)]
    public class SpellBaseCriticalDmg : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            target.UnitData.UnitStatus.GrowthCriticalDamageRate += option.Value2;

            return new ProtoSkillEmpty()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_CRITICAL_DMG,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseCriticalDmg();
        }
    }
}