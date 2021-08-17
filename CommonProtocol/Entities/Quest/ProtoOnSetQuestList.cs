using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSetQuestList : BaseProtocol
    {
        [Key(1)]
        public List<ProtoQuestInfo> QuestInfoList;
    }
}
