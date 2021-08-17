using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetChattingMessage : BaseProtocol
    {
        [Key(1)]
        public List<ProtoChattingMessage> ChattingMessages = new List<ProtoChattingMessage>();
    }


    [MessagePackObject]
    public class ProtoChattingMessage
    {
        [Key(0)]
        public uint Seq;

        [Key(1)]
        public long ChattingDateTime;

        [Key(2)]
        public ChattingType ChattingType;

        [Key(3)]
        public string ChattingMessage;
    }
}
