using System.Collections.Generic;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.UnitTypeFalse)]
    public class SkillConditionUnitTypeFalse : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var unitType = value.CastToEnum<UnitType>();
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].UnitData.UnitIdentifier.UnitType == unitType)
                    return false;
            }

            return true;
        }
    }
}