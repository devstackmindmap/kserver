using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSeasonReward : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public List<ProtoItemResult> ItemResults;

        [Key(3)]
        public int BeforeSeasonRankPoint;

        [Key(4)]
        public Dictionary<uint, int> UnitsBeforeSeasonRankPoint = new Dictionary<uint, int>();

        [Key(5)]
        public List<uint> EnableSeasonPassList;
    }
}
