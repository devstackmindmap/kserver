using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoPubsubNotice : BaseProtocol
    {
        [Key(1)]
        public string NoticeMessage;

    }
}
