using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol.PubSub
{
    [MessagePackObject]
    public class ProtoWeb2OneUpdateQuest : ProtoWeb2One
    {
        [Key(2)]
        public List<ProtoQuestInfo> UpdatedQuestList;
    }
}
