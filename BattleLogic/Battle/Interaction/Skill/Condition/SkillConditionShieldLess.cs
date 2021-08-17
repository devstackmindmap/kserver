using AkaEnum;
using AkaUtility;
using System.Collections.Generic;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.ShieldLess)]
    public class SkillConditionShieldLess : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var shield = value.ToFloat();
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].Shield >= shield)
                    return false;
            }

            return true;
        }
    }
}