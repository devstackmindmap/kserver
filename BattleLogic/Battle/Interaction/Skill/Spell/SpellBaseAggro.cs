using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_AGGRO)]
    public class SpellBaseAggro : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            target.UnitData.UnitStatus.GrowthAggro += (int)option.Value2;

            return new ProtoIncreaseAggro()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_AGGRO,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                AddAggro = (int)option.Value2
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseAggro();
        }
    }
}