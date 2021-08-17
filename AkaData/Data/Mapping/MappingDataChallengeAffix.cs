using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataChallengeAffix : ClassMap<DataChallengeAffix>
    {
        public MappingDataChallengeAffix()
        {
            Map(m => m.Season);
            Map(m => m.NormalAffixIdList).ConvertUsing(row =>
            {
                var data = row.GetField("NormalAffixIdList");
                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.HardAffixIdList).ConvertUsing(row =>
            {
                var data = row.GetField("HardAffixIdList");
                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
