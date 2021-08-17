using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoClanRecommend : BaseProtocol
    {
        [Key(1)]
        public List<ProtoRecommendClanInfo> ProtoClanInfos = new List<ProtoRecommendClanInfo>();
    }
}
