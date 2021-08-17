using System.Collections.Generic;

namespace AkaData
{
    public class DataRoguelikeSaveDeck
    {
        public uint RoguelikeSaveDeckId { get; set; }
        public uint ProposalTreasureId { get; set; }
        public IList<uint> UnitIdList { get; set; }
        public IList<uint> CardStatIdList { get; set; }
        public IList<IList<uint>> ProposalCardStatList { get; set; }
    }
}
