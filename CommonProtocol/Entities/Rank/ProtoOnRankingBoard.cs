using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnRankingBoard : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoRankInfo> RankingBoard = new Dictionary<uint, ProtoRankInfo>();
    }
}
