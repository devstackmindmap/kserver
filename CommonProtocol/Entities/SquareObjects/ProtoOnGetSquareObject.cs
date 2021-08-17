
using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetSquareObject : BaseProtocol
    {
        [Key(1)]
        public SquareObjectResponseType Result { get; set; }

        [Key(2)]
        public ProtoSquareObject SquareObjectInfo { get; set; }

    }
}
