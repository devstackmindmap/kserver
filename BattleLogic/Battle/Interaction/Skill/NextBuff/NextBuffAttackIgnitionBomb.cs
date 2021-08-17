using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_ATTACK_IGNITION_BOMB)]
    public class NextBuffAttackIgnitionBomb : NextBuff
    {
        public override void Add(float value)
        {
        }

        public override void DoSkill(DataSkillOption option, int animationLength, int takeDamageTime)
        {
            base.DoSkill(option, animationLength, takeDamageTime);

            _originValue = option.Value2;
            _currentValue = _originValue;
        }

        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
            if (target.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_IGNITION) == null)
                return;

            performer.BattleHelper.BattleProgress.EnqueueIgnitionBomb(new BattleActionIgnitionBomb(performer, target, _currentValue));
        }

        public override INextBuff Clone()
        {
            return new NextBuffAttackIgnitionBomb();
        }
    }
}