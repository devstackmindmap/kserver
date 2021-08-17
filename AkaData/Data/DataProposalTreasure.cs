using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataProposalTreasure
    {
        public uint ProposalTreasureId { get; set; }
        public IList<IList<uint>> TreasureIdList { get; set; }
    }
}