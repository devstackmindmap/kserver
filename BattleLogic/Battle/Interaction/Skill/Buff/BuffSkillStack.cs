using AkaData;
using AkaEnum;

namespace BattleLogic
{
    // 스택 곱연산
    [Skill(SkillEffectType.BUFF_STATE_IGNITION)]
    public class BuffSkillStack : BuffSkill
    {
        private Unit _target;
        private int _stack;
        private int _value3;
        private float _value2;

        public override float Value => _stack;

        public override void AddEndTime(int milliseconds)
        {
            base.AddEndTime(milliseconds);

            _stack += _value3;
        }

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _target = target;
            _value2 = option.Value2;
            _value3 = option.Value3;
            _stack = option.Value3;

            _target.UnitBuffs.AddTimer(SkillEffectType, _maintainMilliSeconds, EnqueueBuffEnd);
        }

        public override void UpdateSkillOption(DataSkillOption option)
        {
            _value2 = option.Value2;
            _value3 = option.Value3;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillStack();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value *= _stack * _value2;
        }

        private void EnqueueBuffEnd()
        {
            _unit.EnqueueBuffEnd(this);
        }

        public override void MultipleStack(float rate)
        {
            _stack = (int)(_stack * rate);
        }
    }
}