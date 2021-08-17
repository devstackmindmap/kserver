using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetQuestList : BaseProtocol
    {
        [Key(1)]
        public uint UserId;
    }
}
