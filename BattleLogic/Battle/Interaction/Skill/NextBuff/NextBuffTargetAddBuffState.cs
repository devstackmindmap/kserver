using System.Collections.Generic;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_TARGET_ADD_BUFF_STATE)]
    public sealed class NextBuffTargetAddBuffState : NextBuff
    {
        public readonly List<uint> SkillOptionIds = new List<uint>();

        public override void Add(float value)
        {
            SkillOptionIds.Add((uint)value);
        }

        public override void DoSkill(DataSkillOption option, int animationLength, int takeDamageTime)
        {
            base.DoSkill(option, animationLength, takeDamageTime);

            SkillOptionIds.Add((uint)option.Value2);
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
        }

        public override INextBuff Clone()
        {
            return new NextBuffTargetAddBuffState();
        }
    }
}