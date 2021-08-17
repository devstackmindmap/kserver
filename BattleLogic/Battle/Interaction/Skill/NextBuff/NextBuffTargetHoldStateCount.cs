using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_TARGET_HOLD_STATE_COUNT)]
    public class NextBuffTargetHoldStateCount : NextBuff
    {
        private TargetGroupType _targetGroupType;

        public override void DoSkill(DataSkillOption option, int animationLength, int takeDamageTime)
        {
            base.DoSkill(option, animationLength, takeDamageTime);

            _originValue = option.Value2;
            _currentValue = option.Value2;
            _targetGroupType = (TargetGroupType)option.Value3;
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
            var count = 0;
            switch (_targetGroupType)
            {
                case TargetGroupType.Ally:
                    count = GetBuffStateCount(performer, delay);
                    break;
                case TargetGroupType.Enemy:
                    count = GetBuffStateCount(target, delay);
                    break;
            }

            value += value * count * _currentValue;
        }

        private int GetBuffStateCount(Unit unit, double delay)
        {
            var count = 0;
            foreach (var buff in unit.UnitBuffs.Buffs)
            {
                if (buff.Value.IsValid(delay) == false)
                    continue;

                count++;
            }

            return count;
        }

        public override INextBuff Clone()
        {
            return new NextBuffTargetHoldStateCount();
        }
    }
}