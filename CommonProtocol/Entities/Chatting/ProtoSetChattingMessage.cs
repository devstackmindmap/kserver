using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSetChattingMessage : BaseProtocol
    {
        [Key(1)]
        public string UserNickname;

        [Key(2)]
        public string TargetNickname;

        [Key(3)]
        public string ChattingKey;

        [Key(4)]
        public ChattingType ChattingType;

        [Key(5)]
        public string ChattingMessage;
    }
}
