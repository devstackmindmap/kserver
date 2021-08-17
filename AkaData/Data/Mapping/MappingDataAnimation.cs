using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataAnimation : ClassMap<DataAnimation>
    {
        public MappingDataAnimation()
        {
            Map(m => m.AnimationId);
            Map(m => m.AnimationTypes).ConvertUsing(row =>
            {
                return row.GetField<string>("AnimationTypes").CastToDictionary<string, uint>();
            });
        }
    }
}
