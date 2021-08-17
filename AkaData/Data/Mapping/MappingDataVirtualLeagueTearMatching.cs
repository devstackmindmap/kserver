using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataVirtualLeagueTearMatching : ClassMap<DataVirtualLeagueTearMatching>
    {
        public MappingDataVirtualLeagueTearMatching()
        {
            Map(m => m.VirtualLeagueMatchingId);
            Map(m => m.TeamRankPointForMatching);
            Map(m => m.StageRoundIdList).ConvertUsing(row =>
            {
                var data = row.GetField("StageRoundIdList");

                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
