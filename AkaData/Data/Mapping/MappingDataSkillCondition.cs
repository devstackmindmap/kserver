using AkaEnum;
using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataSkillCondition : ClassMap<DataSkillCondition>
    {
        public MappingDataSkillCondition()
        {
            Map(m => m.SkillConditionId);
            Map(m => m.TargetGroupType);
            Map(m => m.TargetType);
            Map(m => m.TargetId);
            Map(m => m.SkillConditionTypeList).ConvertUsing(row =>
            {
                var data = row.GetField("SkillConditionTypeList");
                return data.CastToList(value => (SkillConditionType)value.ToInt());
            });
            Map(m => m.SkillConditionValueList).ConvertUsing(row =>
            {
                var data = row.GetField("SkillConditionValueList");
                return data.CastToList(value => value);
            });
        }
    }
}