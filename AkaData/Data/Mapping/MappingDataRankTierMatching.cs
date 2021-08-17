using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataRankTierMatching : ClassMap<DataRankTierMatching>
    {
        public MappingDataRankTierMatching()
        {
            Map(m => m.RankTierMatchingId);
            Map(m => m.TeamRankPointForMatching);
            Map(m => m.Priority);
            Map(m => m.StageRoundIdList).ConvertUsing(row =>
            {
                var data = row.GetField("StageRoundIdList");

                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.AiMatchingWaitingMillisecond);
        }
    }
}
