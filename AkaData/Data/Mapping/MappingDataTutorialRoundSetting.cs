using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataTutorialRoundSetting : ClassMap<DataTutorialRoundSetting>
    {
        public MappingDataTutorialRoundSetting()
        {
            Map(m => m.Round);
            Map(m => m.StageRoundId);
            Map(m => m.CardIdListOnRound).ConvertUsing(row =>
            {
                var data = row.GetField("CardIdListOnRound");
                return data.CastToList<uint>(uint.Parse);
            });

            Map(m => m.UnitIdListOnRound).ConvertUsing(row =>
            {
                var data = row.GetField("UnitIdListOnRound");
                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
