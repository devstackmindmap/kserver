using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMailReadAll : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public MailType MailType;

        [Key(3)]
        public List<uint> MailIds;
    }
}
