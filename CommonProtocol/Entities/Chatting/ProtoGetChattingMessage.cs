using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetChattingMessage : BaseProtocol
    {
        [Key(1)]
        public string ChattingKey;
    }
}
