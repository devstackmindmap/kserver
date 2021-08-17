using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCardUse : BaseProtocol
    {
        [Key(1)]
        public string RoomId;
        
        [Key(2)]
        public int HandIndex;

        [Key(3)]
        public ProtoTarget Performer;

        [Key(4)]
        public ProtoTarget Target;
    }
}