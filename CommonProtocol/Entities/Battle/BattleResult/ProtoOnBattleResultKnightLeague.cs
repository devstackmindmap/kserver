using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBattleResultRankList : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoOnBattleResultRank> BattleResult;
    }

    [MessagePackObject]
    public class ProtoOnBattleResultRank : ProtoOnBattleResultRankData
    {
        [Key(9)]
        public ProtoNewInfusionBox NewInfusionBox; 
        
        [Key(10)]
        public ProtoFriendInfo RecommendFriend; 
    }
}
