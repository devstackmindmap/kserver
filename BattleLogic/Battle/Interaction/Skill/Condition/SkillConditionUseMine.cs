using System.Collections.Generic;
using AkaEnum;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.UseMine)]
    public class SkillConditionUseMine : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].PlayerType != performer.PlayerType)
                    return false;

                if (targets[i].UnitData.UnitIdentifier.UnitId == performer.UnitData.UnitIdentifier.UnitId)
                    return true;
            }

            return false;
        }
    }
}