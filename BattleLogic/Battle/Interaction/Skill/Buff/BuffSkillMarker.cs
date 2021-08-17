using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_MARKER)]
    public sealed class BuffSkillMarker : BuffSkill
    {
        private Unit _target;
        private int _stack;
        private float _value2;
        private int _value3;

        public override float Value => _stack;

        public override void AddEndTime(int milliseconds)
        {
            base.AddEndTime(milliseconds);

            _stack += _value3;
            CheckStackCharging();
        }

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _target = target;
            _value2 = option.Value2;
            _value3 = option.Value3;
            _stack = option.Value3;

            _target.UnitBuffs.AddTimer(SkillEffectType, _maintainMilliSeconds, EnqueueBuffEnd);

            CheckStackCharging();
        }

        public override void UpdateSkillOption(DataSkillOption option)
        {
            _value2 = option.Value2;
            _value3 = option.Value3;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillMarker();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }

        private void EnqueueBuffEnd()
        {
            _unit.EnqueueBuffEnd(this);
        }

        private void CheckStackCharging()
        {
            var actionCount = (int)Data.GetConstant(DataConstantType.MARKER_ACTION_COUNT).Value;
            if (_stack < actionCount)
                return;

            _stack = 0;
            _unit.BattleHelper.BattleProgress.EnqueueMarkerShock(new BattleActionMarkerShock(_unit, _target, _value2));
            _target.EnqueueBuffEnd(this);
        }

        public override void MultipleStack(float rate)
        {
            _stack = (int)(_stack * rate);

            CheckStackCharging();
        }
    }
}