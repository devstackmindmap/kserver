using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnRankingBoardClan : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoRankInfoClan> RankingBoard = new Dictionary<uint, ProtoRankInfoClan>();

        [Key(2)]
        public ResultType ResultType;
    }
}
