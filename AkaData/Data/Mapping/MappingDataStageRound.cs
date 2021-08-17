using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataStageRound : ClassMap<DataStageRound>
    {
        public MappingDataStageRound()
        {
            Map(m => m.StageRoundId);
            Map(m => m.StageLevelId);
            Map(m => m.Round);
            Map(m => m.MonsterGroupIdList).ConvertUsing(row =>
            {
                var data = row.GetField("MonsterGroupIdList");

                return data.CastToList<uint>(uint.Parse);
            });
            Map(m => m.StartDialogFileName);
            Map(m => m.EndDialogFileName);
            Map(m => m.BackgroundImageId);
            Map(m => m.EndConditionType);
        }
    }
}
