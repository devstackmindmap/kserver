using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_SPECIAL_ATK_AS_INCREASE)]
    public struct SpellSpecialAtkAsIncrease : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            target.UnitData.UnitStatus.Atk = target.UnitData.UnitStatus.Atk;

            return new ProtoIncreaseAtk()
            {
                SkillEffectType = SkillEffectType.SPELL_SPECIAL_ATK_AS_INCREASE,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                AddAtk = target.UnitData.UnitStatus.GrowthAtk
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellSpecialAtkAsIncrease();
        }
    }
}