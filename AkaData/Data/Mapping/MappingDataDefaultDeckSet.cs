using AkaEnum;
using AkaUtility;
using CsvHelper.Configuration;

namespace AkaData
{
   
    public class MappingDataDefaultDeckSet : ClassMap<DataDefaultDeckSet>
    {
        public MappingDataDefaultDeckSet()
        {
            Map(m => m.ModeType);

            Map(m => m.CardIdList).ConvertUsing(row =>
            {
                var data = row.GetField("CardIdList");
                return data.CastToList<uint>(uint.Parse);
            });

            Map(m => m.UnitIdList).ConvertUsing(row =>
            {
                var data = row.GetField("UnitIdList");
                return data.CastToList<uint>(uint.Parse);
            });
        }
    }
}
