
using AkaEnum;
using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSquareObjectBuyEnergy : BaseProtocol
    {
        [Key(1)]
        public SquareObjectResponseType Result { get; set; }

        [Key(2)]
        public int RemainedGem { get; set; }

        [Key(3)]
        public DateTime InjectedTime { get; set; }
    }
}
