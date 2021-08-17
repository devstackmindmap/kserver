using AkaUtility;
using CsvHelper.Configuration;
using System.Collections.Generic;

namespace AkaData
{
    public class MappingDataRoguelikeSaveDeck : ClassMap<DataRoguelikeSaveDeck>
    {
        public MappingDataRoguelikeSaveDeck()
        {
            Map(m => m.RoguelikeSaveDeckId);
            Map(m => m.ProposalTreasureId);
            
            Map(m => m.UnitIdList).ConvertUsing(row =>
            {
                var data = row.GetField<string>("UnitIdList");
                return data.CastToList<uint>(uint.Parse);
            });

            Map(m => m.CardStatIdList).ConvertUsing(row =>
            {
                var data = row.GetField<string>("CardStatIdList");
                return data.CastToArray<uint>(uint.Parse);
            });

            Map(m => m.ProposalCardStatList).ConvertUsing(row =>
            {
                int index = 0;
                var proposalCardStatList = new List<IList<uint>>();

                while (row.TryGetField<string>($"ProposalCardStatList{index.ToString()}", out var data))
                {
                    index++;
                    if (string.IsNullOrEmpty(data) || data.Equals("0"))
                        proposalCardStatList.Add(new List<uint>());
                    else
                        proposalCardStatList.Add(data.CastToList<uint>(uint.Parse));
                }
                return proposalCardStatList;
            });
        }
    }
}
