using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEvent
    {
        [Key(0)]
        public EventType EventType;

        [Key(1)]
        public long StartDateTime;

        [Key(2)]
        public long EndDateTime;
    }
}
