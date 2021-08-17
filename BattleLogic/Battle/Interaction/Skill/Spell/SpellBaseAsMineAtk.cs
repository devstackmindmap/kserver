using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_BASE_AS_MINE_ATK)]
    public class SpellBaseAsMineAtk : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var attackerAtk = BuffStateCalculator.GetAtkWithConditions(target.UnitData.UnitStatus.Atk, target, SkillEffectTypes.NormalAttackAtkByAttackers, 0);

            var addAtk = (int) (attackerAtk * option.Value2);
            target.UnitData.UnitStatus.GrowthAtk += addAtk;

            return new ProtoIncreaseAtk()
            {
                SkillEffectType = SkillEffectType.SPELL_BASE_AS_MINE_ATK,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                AddAtk = addAtk
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellBaseAsMineAtk();
        }
    }
}