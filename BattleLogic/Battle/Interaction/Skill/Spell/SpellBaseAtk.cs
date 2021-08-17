using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_ATK)]
    public class SpellBaseAtk : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            target.UnitData.UnitStatus.GrowthAtk += (int)option.Value2;

            return new ProtoIncreaseAtk()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_ATK,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                AddAtk = (int)option.Value2
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseAtk();
        }
    }
}