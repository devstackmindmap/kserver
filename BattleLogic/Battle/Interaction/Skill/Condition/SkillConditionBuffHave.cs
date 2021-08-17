using System.Collections.Generic;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.BuffStateHave)]
    public class SkillConditionBuffHave : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var skillType = value.CastToEnum<SkillEffectType>();
            for (var i = 0; i < targets.Count; i++)
            {
                var buff = targets[i].GetConditionBuffSkill(skillType);
                if (buff == null || buff.IsValid() == false)
                    return false;
            }

            return true;
        }
    }
}