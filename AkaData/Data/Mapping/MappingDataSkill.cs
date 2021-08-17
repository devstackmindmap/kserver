using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataSkill : ClassMap<DataSkill>
    {
        public MappingDataSkill()
        {
            Map(m => m.SkillId);
            Map(m => m.SkillType);
            Map(m => m.SkillOptionList).ConvertUsing(row =>
            {
                var options = row.GetField("SkillOptionList");
                return options.CastToArray<uint>(uint.Parse);
            });

            Map(m => m.AnimationType);
            Map(m => m.UnitInitial);
        }
    }
}
