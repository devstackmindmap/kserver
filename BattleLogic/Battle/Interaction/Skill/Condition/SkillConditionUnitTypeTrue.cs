using AkaEnum;
using AkaUtility;
using System.Collections.Generic;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.UnitTypeTrue)]
    public class SkillConditionUnitTypeTrue : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var unitType = value.CastToEnum<UnitType>();
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].UnitData.UnitIdentifier.UnitType != unitType)
                    return false;
            }

            return true;
        }
    }
}