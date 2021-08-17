using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataMonsterPatternFlow : ClassMap<DataMonsterPatternFlow>
    {
        public MappingDataMonsterPatternFlow()
        {
            Map(m => m.MonsterPatternFlowId);
            Map(m => m.TransMonsterPatternId);
            Map(m => m.MonsterPatternConditionIdList).ConvertUsing(row =>
            {
                var data = row.GetField("MonsterPatternConditionIdList");

                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
