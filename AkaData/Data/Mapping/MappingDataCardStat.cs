using AkaEnum;
using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataCardStat : ClassMap<DataCardStat>
    {
        public MappingDataCardStat()
        {

            Map(m => m.CardStatId);
            Map(m => m.CardId);
            Map(m => m.Level);
            Map(m => m.SkillConditionId);
            Map(m => m.Elixir);
            Map(m => m.HpCost);
            Map(m => m.RequirePieceCountForNextLevelUp);
            Map(m => m.NeedGoldForNextLevelUp);
            Map(m => m.SkillIdList).ConvertUsing(row =>
            {
                var data = row.GetField("SkillIdList");
                return data.CastToList<uint>(uint.Parse);
            });

        }
    }
}
