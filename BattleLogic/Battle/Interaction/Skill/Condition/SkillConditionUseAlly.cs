using System.Collections.Generic;
using AkaEnum;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.UseAlly)]
    public class SkillConditionUseAlly : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            for (var i = 0; i < targets.Count; i++)
            {
                if (performer.PlayerType != targets[i].PlayerType)
                    return false;
            }

            return true;
        }
    }
}