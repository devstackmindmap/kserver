
using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSquareObjectPowerInject : ProtoOnGetSquareObject
    {
        [Key(3)]
        public List<ProtoItemResult> RewardList { get; set; }
    }
}
