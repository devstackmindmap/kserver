using System.Collections.Generic;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.HpOverAndEqual)]
    public class SkillConditionHpOverAndEqual : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var hp = value.ToInt();
            for (var i = 0; i < targets.Count; i++)
            {
                if (targets[i].UnitData.UnitStatus.Hp < hp)
                    return false;
            }

            return true;
        }
    }
}