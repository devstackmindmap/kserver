using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoTrigger2OneSquareObject : ProtoWeb2One
    {
        [Key(2)]
        public ProtoSquareObject SquareObjectInfo;
    }
}
