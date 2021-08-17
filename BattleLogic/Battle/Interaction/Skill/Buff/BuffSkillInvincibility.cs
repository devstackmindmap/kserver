using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_INVINCIBILITY)]
    public sealed class BuffSkillInvincibility : BuffSkill
    {
        private int _currentCount;
        private int _originCount;

        public override float Value => 0;

        public override bool IsValid(double delayMilliseconds = 0)
        {
            if (_originCount > 0 && _currentCount <= 0)
                return false;

            return base.IsValid(delayMilliseconds);
        }

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _originCount = option.Value3;
            _currentCount = option.Value3;
        }

        public override void DecreaseCount()
        {
            _currentCount--;
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value = 0;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillInvincibility();
        }
    }
}