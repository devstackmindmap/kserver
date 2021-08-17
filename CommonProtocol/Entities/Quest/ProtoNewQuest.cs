using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoNewQuest : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public QuestType QuestType;

        [Key(3)]
        public uint TargetQuestGroupId;
    }
}
