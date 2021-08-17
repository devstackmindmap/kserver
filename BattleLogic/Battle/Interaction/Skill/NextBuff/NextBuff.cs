using AkaData;
using AkaEnum;

namespace BattleLogic
{
    public abstract class NextBuff : INextBuff
    {
        protected float _originValue;
        protected float _currentValue;

        public int AnimationLength { get; private set; }
        public int TakeDamageTime { get; private set; }
        public SkillEffectType SkillEffectType { get; private set; }
        public DataSkillOption DataSkillOption { get; private set; }

        public virtual void Add(float value)
        {
            _currentValue += value;
        }

        public virtual void DoSkill(DataSkillOption option, int animationLength, int takeDamageTime)
        {
            SkillEffectType = option.SkillEffectType;
            AnimationLength = animationLength;
            TakeDamageTime = takeDamageTime;
            DataSkillOption = option;
        }

        public abstract void CalculateValue(ref float value, Unit target, Unit performer, double delay);
        public abstract INextBuff Clone();
    }
}