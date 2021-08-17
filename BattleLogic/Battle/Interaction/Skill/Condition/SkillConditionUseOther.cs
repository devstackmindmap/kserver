using System.Collections.Generic;
using AkaEnum;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.UseOther)]
    public class SkillConditionUseOther : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            for (var i = 0; i < targets.Count; i++)
            {
                if (performer.PlayerType != targets[i].PlayerType || performer.UnitData.UnitIdentifier.UnitId != targets[i].UnitData.UnitIdentifier.UnitId)
                    return true;
            }

            return false;
        }
    }
}