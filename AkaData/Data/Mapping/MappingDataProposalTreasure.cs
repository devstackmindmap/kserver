using AkaUtility;
using CsvHelper.Configuration;
using System.Collections.Generic;

namespace AkaData
{
    public class MappingDataProposalTreasure : ClassMap<DataProposalTreasure>
    {
        public MappingDataProposalTreasure()
        {
            Map(m => m.ProposalTreasureId);
            
            Map(m => m.TreasureIdList).ConvertUsing(row =>
            {
                int index = 0;
                var treasureIdList = new List<IList<uint>>();

                while (row.TryGetField<string>($"TreasureIdList{index.ToString()}", out var data))
                {
                    index++;
                    if (string.IsNullOrEmpty(data) || data.Equals("0"))
                        treasureIdList.Add(new List<uint>());
                    else
                        treasureIdList.Add(data.CastToList(uint.Parse));
                }
                return treasureIdList;
            });
        }
    }
}
