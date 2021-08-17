
using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSquareObjectStop : ProtoOnGetSquareObject
    {
        [Key(3)]
        public List<ProtoItemResult> RewardList { get; set; }
    }
}
