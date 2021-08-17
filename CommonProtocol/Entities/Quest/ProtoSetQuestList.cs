using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSetQuestList : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public List<ProtoQuestInfo> QuestInfoList;
    }
}
