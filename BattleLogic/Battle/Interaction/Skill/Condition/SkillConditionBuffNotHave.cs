using System.Collections.Generic;
using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    [SkillCondition(SkillConditionType.BuffStateNotHave)]
    public class SkillConditionBuffNotHave : BaseSkillCondition
    {
        public override bool IsConditionPass(Unit performer, List<Unit> targets, SkillConditionType conditionType, string value)
        {
            var skillType = value.CastToEnum<SkillEffectType>();
            for (var i = 0; i < targets.Count; i++)
            {
                var buff = targets[i].GetConditionBuffSkill(skillType);
                if (buff != null && buff.IsValid())
                    return false;
            }

            return true;
        }
    }
}