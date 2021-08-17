using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataReward : ClassMap<DataReward>
    {
        public MappingDataReward()
        {
            Map(m => m.RewardId);
            Map(m => m.ItemIdList).ConvertUsing(row =>
            {
                var data = row.GetField<string>("ItemIdList");

                return data.CastToArray<uint>(uint.Parse);
            });
            Map(m => m.NeedEnergy);
        }
    }
}
