using System;
using AkaData;
using AkaEnum;
using CommonProtocol;

namespace BattleLogic
{
    [Skill(SkillEffectType.SPELL_SPECIAL_BUFF_STACK_COUNT)]
    public class SpellSpecialBuffStackCount : ISpellSkill
    {
        public BaseSkillProto DoSkill(DataSkillOption option, Unit performer, Unit target, Card card, Skill skill, DateTime nowDateTime, bool isCritical)
        {
            var targetEffectType = (SkillEffectType)option.Value3;
            var stack = 0;
            switch (targetEffectType)
            {
                case SkillEffectType.BUFF_STATE_POISON:
                    target.MultiplePoison(option.Value2);
                    stack = target.GetPoisonStack();
                    break;
                default:
                    var buff = target.UnitBuffs.GetBuffSkill(targetEffectType);
                    if (buff?.IsValid() == true)
                    {
                        buff.MultipleStack(option.Value2);
                        stack = (int)buff.Value;
                    }
                    break;
            }

            return new ProtoSpellBuffStackCount()
            {
                SkillEffectType = option.SkillEffectType,
                TargetPlayerType = target.PlayerType,
                TargetUnitId = target.UnitData.UnitIdentifier.UnitId,
                Stack = stack
            };
        }

        public ISpellSkill Clone()
        {
            return new SpellSpecialBuffStackCount();
        }
    }
}