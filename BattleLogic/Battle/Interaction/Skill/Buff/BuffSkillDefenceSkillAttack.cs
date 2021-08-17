using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_DEFENCE_SKILL_ATTACK, SkillEffectType.BUFF_STATE_CURSER)]
    public class BuffSkillDefenceSkillAttack : BuffSkill
    {
        private int _currentCount;
        private int _originCount;
        private float _value;

        public override float Value => _value;

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
            _value = option.Value2;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillDefenceSkillAttack();
        }

        public override void DecreaseCount()
        {
            _currentCount--;
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value -= value * _value;
        }
    }
}