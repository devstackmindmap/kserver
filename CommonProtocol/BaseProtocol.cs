using CommonProtocol.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class BaseProtocol
    {
        [Key(0)]
        public MessageType MessageType;
    }
}
