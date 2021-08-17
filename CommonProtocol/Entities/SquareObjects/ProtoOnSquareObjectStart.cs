
using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSquareObjectStart : BaseProtocol
    {
        [Key(1)]
        public SquareObjectResponseType Result { get; set; }

        [Key(2)]
        public ProtoSquareObject SquareObjectInfo { get; set; }

        [Key(3)]
        public int RemainTicket { get; set; }
    }
}
