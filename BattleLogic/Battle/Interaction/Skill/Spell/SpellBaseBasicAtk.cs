using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_BASIC_ATK)]
    public sealed class SpellBaseBasicAtk : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var addAtk = (int)(target.UnitData.UnitStatus.BasicAtk * option.Value2);
            target.UnitData.UnitStatus.GrowthAtk += addAtk;

            return new ProtoIncreaseAtk()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_BASIC_ATK,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                AddAtk = addAtk
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseBasicAtk();
        }
    }
}