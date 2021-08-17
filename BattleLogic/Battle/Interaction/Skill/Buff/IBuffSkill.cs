using AkaEnum;
using System;
using AkaData;

namespace BattleLogic
{
    public interface IBuffSkill
    {
        uint SkillOptionId { get; }
        DateTime BuffStartTime { get; }
        float Value { get; }
        bool IsValid(double delayMilliseconds = 0d);
        void AddBulletTime(int milliseconds);
        void AddEndTime(int milliseconds);
        void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime);
        void BuffEnd();
        void DecreaseCount();
        void CalculateValue(ref float value, Unit target);
        void UpdateSkillOption(DataSkillOption option);
        int RemainCount { get; }
        SkillEffectType SkillEffectType { get; }
        DateTime EndDateTime { get; }
        IBuffSkill Clone();
        void UpdateBuffStartTime();
        void MultipleStack(float rate);
    }
}
