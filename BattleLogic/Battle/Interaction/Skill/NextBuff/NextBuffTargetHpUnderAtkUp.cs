using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_TARGET_HP_UNDER_ATK_UP)]
    public sealed class NextBuffTargetHpUnderAtkUp : NextBuff
    {
        private float _hpRate;

        public override void DoSkill(DataSkillOption option, int animationLengh, int takeDamageTime)
        {
            base.DoSkill(option, animationLengh, takeDamageTime);

            _hpRate = option.Value1;
            _originValue = option.Value2;
            _currentValue = _originValue;
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
            var targetHp = target.UnitData.UnitStatus.MaxHp * _hpRate;
            if (target.UnitData.UnitStatus.Hp > targetHp)
                return;

            value *= _currentValue;
        }

        public override INextBuff Clone()
        {
            return new NextBuffTargetHpUnderAtkUp();
        }
    }
}