using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{

    [MessagePackObject]
    public class ProtoOnSkipCurrentQuest : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public List<ProtoQuestInfo> QuestInfoList;

        [Key(3)]
        public Dictionary<MaterialType, int> RemainedMaterial;

    }
}
