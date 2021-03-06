using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_ATTACK_ATK_UP, SkillEffectType.BUFF_NEXT_ATTACK_ATK_UP_AND_ALL_TARGET)]
    public sealed class NextBuffAttackAtkUp : NextBuff
    {
        public override void DoSkill(DataSkillOption option, int animationLengh, int takeDamageTime)
        {
            base.DoSkill(option, animationLengh, takeDamageTime);

            _originValue = option.Value2;
            _currentValue = _originValue;
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
            value *= _currentValue;
        }

        public override INextBuff Clone()
        {
            return new NextBuffAttackAtkUp();
        }
    }
}