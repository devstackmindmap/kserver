using System.Collections.Generic;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.HpLessRate)]
    public class SkillConditionHpLessRate : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var rate = value.ToFloat();
            for (var i = 0; i < targets.Count; i++)
            {
                var hp = targets[i].UnitData.UnitStatus.MaxHp * rate;
                if (targets[i].UnitData.UnitStatus.Hp >= hp)
                    return false;
            }

            return true;
        }
    }
}