using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_SHIELD)]
    public class SpellShield : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var shield = target.AddShield(card, option, skill.AnimationData.BulletTime);

            return new ProtoSpellShield()
            {
                SkillEffectType = SkillEffectType.SPELL_SHIELD,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                Shield = shield.Shield,
                StartTime = shield.StartTime.Ticks,
                EndTime = shield.EndTime.Ticks,
                CardId = card.CardId
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellShield();
        }
    }
}
