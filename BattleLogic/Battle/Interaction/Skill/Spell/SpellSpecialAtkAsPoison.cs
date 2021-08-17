using AkaData;
using AkaEnum;
using CommonProtocol;
using System;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_SPECIAL_ATK_AS_POISON)]
    public class SpellSpecialAtkAsPoison : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var poisonStack = target.GetPoisonStack();
            var increaseAtk = (int)(poisonStack * option.Value2);
            target.UnitData.UnitStatus.GrowthAtk += increaseAtk;

            return new ProtoIncreaseAtk()
            {
                SkillEffectType = SkillEffectType.SPELL_SPECIAL_ATK_AS_POISON,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                AddAtk = increaseAtk
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellSpecialAtkAsPoison();
        }
    }
}