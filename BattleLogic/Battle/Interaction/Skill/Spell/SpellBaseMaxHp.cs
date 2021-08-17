using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_MAX_HP)]
    public class SpellBaseMaxHp : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var growthMaxHp = (int)(target.UnitData.UnitStatus.MaxHp * option.Value2);
            target.UnitData.UnitStatus.GrowthMaxHp += growthMaxHp;
            target.UnitData.UnitStatus.Hp = target.UnitData.UnitStatus.MaxHp;

            return new ProtoGrowthMaxHp()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_MAX_HP,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                GrowthMaxHp = growthMaxHp
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseMaxHp();
        }
    }
}