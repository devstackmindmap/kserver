using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataUnitStat : ClassMap<DataUnitStat>
    {
        public MappingDataUnitStat()
        {
            Map(m => m.UnitStatId);
            Map(m => m.UnitId);
            Map(m => m.Level);
            Map(m => m.RequirePieceCountForNextLevelUp);
            Map(m => m.AtkSpd);
            Map(m => m.Atk);
            Map(m => m.Hp);
            Map(m => m.CriRate);
            Map(m => m.CriDmgRate);
            Map(m => m.Aggro);
            Map(m => m.NeedGoldForNextLevelUp);
            Map(m => m.PassiveConditionId).ConvertUsing(row =>
            {
                var data = row.GetField("PassiveConditionId");
                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}