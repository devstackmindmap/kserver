using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataSkillCondition
    {
        public uint SkillConditionId { get; set; }
        public TargetGroupType TargetGroupType { get; set; }
        public TargetType TargetType { get; set; }
        public uint TargetId { get; set; }
        public List<SkillConditionType> SkillConditionTypeList { get; set; }
        public List<string> SkillConditionValueList { get; set; }
    }
}