using MessagePack;
using AkaEnum;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetStageLevelRoomInfo : BaseProtocol
    {
        [Key(1)]
        public uint StageLevelId;

        [Key(2)]
        public uint ClearRound;

        [Key(3)]
        public List<uint> CardStatIdList;

        [Key(4)]
        public List<uint> ReplaceCardStatIdList;

        [Key(5)]
        public List<uint> ProposalTreasureIdList;

        [Key(6)]
        public List<uint> TreasureIdList;
    }
}
