using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnNewQuest : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, uint> QuestGroupAndDynamicList;

        [Key(2)]
        public int RefreshCount;

        [Key(3)]
        public int AddCount;

        [Key(4)]
        public ResultType ResultType;

        [Key(5)]
        public MaterialType MaterialType;

        [Key(6)]
        public int RemainedMaterials;
    }
}
