using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataStageLevelFlow : ClassMap<DataStageLevelFlow>
    {
        public MappingDataStageLevelFlow()
        {
            Map(m => m.StageLevelId);
            Map(m => m.OpenStageIdList).ConvertUsing(row =>
            {
                var datas = row.GetField<string>("OpenStageIdList");
                return datas.CastToArray<uint>(uint.Parse);
            });
        }
    }
}
