using System;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    // 스택 합연산
    [Skill(SkillEffectType.BUFF_STATE_ACCUMULATE_NORMAL_ATTACK, SkillEffectType.BUFF_STATE_ACCUMULATE_SKILL_ATTACK)]
    public sealed class BuffSkillStackSum : BuffSkill
    {
        private Unit _target;
        private int _stack;
        private int _value3;
        private float _value2;

        public override float Value => _stack;

        public override void AddEndTime(int milliseconds)
        {
            base.AddEndTime(milliseconds);

            _stack = Math.Min(_stack + _value3, (int)Data.GetConstant(DataConstantType.ACCUMULATE_MAX_COUNT).Value);
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
            return new BuffSkillStackSum();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value += value * _stack * _value2;
        }

        private void EnqueueBuffEnd()
        {
            _unit.EnqueueBuffEnd(this);
        }

        public override void MultipleStack(float rate)
        {
            _stack = Math.Min((int)(_stack * rate), (int)Data.GetConstant(DataConstantType.ACCUMULATE_MAX_COUNT).Value);
        }
    }
}