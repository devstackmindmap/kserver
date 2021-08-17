using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_ADD_DMG_NORMAL_ATTCK)]
    public class SpellBaseAddDamageNormalAttack : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            performer.UnitData.UnitStatus.AddDamageRateToNormalAttack *= option.Value2;

            return new ProtoSkillEmpty()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_ADD_DMG_NORMAL_ATTCK,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseAddDamageNormalAttack();
        }
    }
}