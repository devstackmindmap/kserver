using AkaData;
using AkaEnum;
using AkaEnum.Battle;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_STEEL, SkillEffectType.BUFF_STATE_STEEL_ALL, SkillEffectType.BUFF_STATE_STEEL_IMMUNE_DISAPPEARANCE)]
    public class BuffSkillSteel : BuffSkill
    {
        private int _currentCount;
        private int _originCount;
        private float _value;

        public override float Value => 0;

        public override bool IsValid(double delayMilliseconds = 0d)
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

        public override void DecreaseCount()
        {
            _currentCount--;
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            if (target == null || target.IsContainNextBuff(SkillEffectType.BUFF_NEXT_ATTACK_IGNORE_STEEL_COUNTER) == false)
                value -= value * _value;

            _unit.MyPlayer.ActionLog.IncreaseStatus(ActionStatusType.BlockingAttack);
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillSteel();
        }
    }
}