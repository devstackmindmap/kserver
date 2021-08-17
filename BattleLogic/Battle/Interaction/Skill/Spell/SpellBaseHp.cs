using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_HP)]
    public class SpellBaseHp : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            target.UnitData.UnitStatus.Hp = (int)(target.UnitData.UnitStatus.MaxHp * option.Value2);

            return new ProtoSetCurrentHp()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_HP,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                CurrentHp = target.UnitData.UnitStatus.Hp
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseHp();
        }
    }
}