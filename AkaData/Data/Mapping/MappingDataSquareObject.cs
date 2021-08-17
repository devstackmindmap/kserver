using AkaUtility;
using CsvHelper.Configuration;
using System.Collections.Generic;

namespace AkaData
{
    public class MappingDataSquareObject : ClassMap<DataSquareObject>
    {
        public MappingDataSquareObject()
        {
            Map(m => m.SquareObjectId);
            Map(m => m.SquareObjectLevel);
            Map(m => m.MaxShield);
            Map(m => m.NeedExpForNextLevelUp);
            Map(m => m.NeedGoldForNextLevelUp);

            Map(m => m.InvasionLvLists).ConvertUsing(row =>
            {
                int index = 0;
                var invasionLevelList = new List<IList<uint>>();

                while (row.TryGetField<string>($"InvasionLv{(++index).ToString()}List", out var data))
                {
                    if (string.IsNullOrEmpty(data) || data.Equals("0"))
                        invasionLevelList.Add(new List<uint>());
                    else
                        invasionLevelList.Add(data.CastToList<uint>(uint.Parse));
                }
                return invasionLevelList;
            });
        }
    }
}
