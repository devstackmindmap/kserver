using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BOSS_PHASE_NOTICE)]
    public sealed class SpellBossPhaseNotice : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            return new ProtoSkillEmpty()
            {
                SkillEffectType = SkillEffectType.SPELL_BOSS_PHASE_NOTICE,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBossPhaseNotice();
        }
    }
}