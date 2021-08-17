using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataMonsterGroup : ClassMap<DataMonsterGroup>
    {
        public MappingDataMonsterGroup()
        {
            Map(m => m.MonsterGroupId);
            Map(m => m.MonsterIdList).ConvertUsing(row =>
            {
                var data = row.GetField("MonsterIdList");

                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
