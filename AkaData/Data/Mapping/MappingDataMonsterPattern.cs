using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataMonsterPattern : ClassMap<DataMonsterPattern>
    {
        public MappingDataMonsterPattern()
        {
            Map(m => m.MonsterPatternId);
            Map(m => m.ActivePatternConditionIdList).ConvertUsing(row =>
            {
                var datas = row.GetField<string>("ActivePatternConditionIdList");
                return datas.CastToList<uint>(uint.Parse);
            });
            Map(m => m.DeactivePatternConditionIdList).ConvertUsing(row =>
            {
                var datas = row.GetField<string>("DeactivePatternConditionIdList");
                return datas.CastToList<uint>(uint.Parse);
            });
            Map(m => m.MonsterPatternConditionIdList).ConvertUsing(row =>
            {
                var datas = row.GetField<string>("MonsterPatternConditionIdList");
                return datas.CastToList<uint>(uint.Parse);
            });
            Map(m => m.RepeatCount);
            Map(m => m.CardStatId);
            Map(m => m.MonsterPatternFlowIdList).ConvertUsing(row =>
            {
                var datas = row.GetField<string>("MonsterPatternFlowIdList");
                return datas.CastToList<uint>(uint.Parse);
            });
        }
    }
}
