using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataMonster : ClassMap<DataMonster>
    {
        public MappingDataMonster()
        {
            Map(m => m.MonsterId);
            Map(m => m.BaseUnitId);
            Map(m => m.MonsterPatternIdList).ConvertUsing(row =>
            {
                var data = row.GetField("MonsterPatternIdList");
                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.MonsterType);
        }
    }
}
