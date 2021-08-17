using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_ATTACK_SHIELD_IGNORE)]
    public sealed class NextBuffAttackShieldIgnore : NextBuff
    {
        public override void DoSkill(DataSkillOption option, int animationLengh, int takeDamageTime)
        {
            base.DoSkill(option, animationLengh, takeDamageTime);

            _originValue = option.Value2;
            _currentValue = _originValue;
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
        }

        public override INextBuff Clone()
        {
            return new NextBuffAttackShieldIgnore();
        }
    }
}