using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoStageRoundResult : BaseProtocol
    {
        [Key(1)]
        public uint StageLevelId;

        [Key(2)]
        public uint Round;

        [Key(3)]
        public List<uint> NextCardStatIdList;

        [Key(4)]
        public List<uint> CardStatIdList;
        
        [Key(5)]
        public List<uint> ProposalTreasureIdList;

        [Key(6)]
        public List<uint> TreasureIdList;

    }
}
