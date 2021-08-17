using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSetStageLevelRoomInfo : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
        
        [Key(2)]
        public BattleType BattleType;

        [Key(3)]
        public uint StageLevelId;

        [Key(4)]
        public uint ClearRound;

        [Key(5)]
        public List<uint> CardStatIdList;

        [Key(6)]
        public List<uint> ReplaceCardStatIdList;

        [Key(7)]
        public List<uint> ProposalTreasureIdList;

        [Key(8)]
        public List<uint> TreasureIdList;

    }
}
