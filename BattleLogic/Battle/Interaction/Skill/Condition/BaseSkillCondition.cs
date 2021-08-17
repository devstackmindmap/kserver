using System.Collections.Generic;
using AkaEnum;

namespace BattleLogic
{
    public abstract class BaseSkillCondition
    {
        public abstract bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value);
    }
}