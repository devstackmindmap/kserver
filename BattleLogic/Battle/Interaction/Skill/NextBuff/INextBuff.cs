using AkaData;

namespace BattleLogic
{
    public interface INextBuff
    {
        int AnimationLength { get; }
        int TakeDamageTime { get; }
        void Add(float value);
        void DoSkill(DataSkillOption option, int animationLength, int takeDamageTime);
        void CalculateValue(ref float value, Unit target, Unit performer, double delay);
        INextBuff Clone();
        DataSkillOption DataSkillOption { get; }
    }
}