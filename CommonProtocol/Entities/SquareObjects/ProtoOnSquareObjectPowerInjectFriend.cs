
using AkaEnum;
using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSquareObjectPowerInjectFriend : BaseProtocol
    {
        [Key(1)]
        public SquareObjectResponseType Result { get; set; }
    }
}
