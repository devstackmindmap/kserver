using System;
using System.Collections.Generic;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_DISAPPEARANCE)]
    public class SpellDisappearance : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var targetType = (StateGoodBadType)option.Value3;
            var removedTypes = new List<SkillEffectType>();
            var isGoodTypeImmune = IsDisappearanceImmune(target);
            var ignoreTypes = GetIgnoreTypes(target);

            switch (targetType)
            {
                case StateGoodBadType.All:
                    if (isGoodTypeImmune)
                        removedTypes = target.RemoveConditionBuffs(ignoreTypes, StateGoodBadType.Bad);
                    else
                        removedTypes = target.RemoveConditionBuffs(ignoreTypes, StateGoodBadType.Good, StateGoodBadType.Bad);
                    break;
                case StateGoodBadType.Good:
                    if (isGoodTypeImmune == false)
                        removedTypes = target.RemoveConditionBuffs(ignoreTypes, StateGoodBadType.Good);
                    break;
                case StateGoodBadType.Bad:
                    removedTypes = target.RemoveConditionBuffs(ignoreTypes, StateGoodBadType.Bad);
                    break;
            }


            return new ProtoSpellDisapearance()
            {
                SkillEffectType = SkillEffectType.SPELL_DISAPPEARANCE,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                RemovedEffectTypes = removedTypes
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellDisappearance();
        }

        private bool IsDisappearanceImmune(Unit target)
        {
            return target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_IMMUNE_DISAPPEARANCE) ||
                   target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_STEEL_IMMUNE_DISAPPEARANCE);
        }

        private List<SkillEffectType> GetIgnoreTypes(Unit target)
        {
            var ignoreTypes = new List<SkillEffectType>();

            if(target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE))
                ignoreTypes.Add(SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE);

            if(target.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_CURSER))
                ignoreTypes.Add(SkillEffectType.BUFF_STATE_CURSER);

            return ignoreTypes;
        }
    }
}