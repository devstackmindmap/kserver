using System;
using AkaEnum;

namespace BattleLogic
{
    public class SkillConditionAttribute : Attribute
    {
        public SkillConditionType ConditionType;

        public SkillConditionAttribute(SkillConditionType conditionType)
        {
            ConditionType = conditionType;
        }
    }
}