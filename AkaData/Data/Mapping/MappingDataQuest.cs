using AkaEnum;
using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataQuest : ClassMap<DataQuest>
    {
        public MappingDataQuest()
        {
            Map(m => m.QuestGroupId);
            Map(m => m.QuestId);
            Map(m => m.Order);
            Map(m => m.QuestType);
            Map(m => m.QuestProcessType);
            Map(m => m.QuestTargetType);
            Map(m => m.ClassId);
            Map(m => m.QuestConditionValue);
            Map(m => m.RewardId);
            Map(m => m.MailRewardId);
            Map(m => m.QuestKeeping).ConvertUsing(row =>
            {
                var data = row.GetField("QuestKeeping");
                int.TryParse(data, out var result);
                return result == 1;
            });
            Map(m => m.SkillEffectTypeList).ConvertUsing(row =>
            {
                  var data = row.GetField("SkillEffectTypeList");
                  return data.CastToList( skillEffectId => (SkillEffectType)skillEffectId.ToInt());
            });
        }
    }
}
