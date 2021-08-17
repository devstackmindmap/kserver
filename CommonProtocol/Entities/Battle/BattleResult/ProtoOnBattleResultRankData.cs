using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBattleResultRankData : ProtoOnBattleResult
    {
        [Key(6)]
        public Dictionary<uint, ProtoRankData> UnitsRankData; 

        [Key(7)]
        public ProtoRankData UserRankData; 
        
        [Key(8)]
        public int ChangedRankPoint; 
    }
}
