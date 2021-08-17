using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataProduct : ClassMap<DataProduct>
    {
        public MappingDataProduct()
        {
            Map(m => m.ProductId);
            Map(m => m.RewardIdList).ConvertUsing(row =>
            {
                var data = row.GetField<string>("RewardIdList");

                return data.CastToArray<uint>(uint.Parse);
            });
        }
    }
}
