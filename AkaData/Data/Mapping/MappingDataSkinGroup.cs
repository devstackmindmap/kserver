using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
    public class MappingDataSkinGroup : ClassMap<DataSkinGroup>
    {
        public MappingDataSkinGroup()
        {
            Map(m => m.SkinGroupId);
            Map(m => m.BaseSkinId);
            Map(m => m.SkinIdList).ConvertUsing(row =>
            {
                var options = row.GetField("SkinIdList");
                return options.CastToList<uint>(uint.Parse);
            });
        }
    }
}
