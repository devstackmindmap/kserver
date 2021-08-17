using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataChallenge : ClassMap<DataChallenge>
    {
        public MappingDataChallenge()
        {

            Map(m => m.Season);
            Map(m => m.Day);
            Map(m => m.DifficultLevelList).ConvertUsing(row =>
            {
                var data = row.GetField("DifficultLevelList");
                return data.CastToList<int>(int.Parse);
            });
            Map(m => m.StageLevelIdList).ConvertUsing(row =>
            {
                var data = row.GetField("StageLevelIdList");
                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.RewardIdList1).ConvertUsing(row =>
            {
                var data = row.GetField("RewardIdList1");
                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.RewardIdList2).ConvertUsing(row =>
            {
                var data = row.GetField("RewardIdList2");
                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.BanCardIdList).ConvertUsing(row =>
            {
                var data = row.GetField("BanCardIdList");
                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
