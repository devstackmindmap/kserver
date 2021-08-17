using MessagePack;
using System;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoQuestInfo : BaseProtocol
    {
        [Key(1)]
        public uint QuestGroupId;


        [Key(2)]
        public uint CompleteOrder;

        [Key(3)]
        public int PerformCount;

        [Key(4)]
        public uint ReceivedOrder;

        [Key(5)]
        public uint DynamicQuestGroupId;

        [Key(6)]
        public DateTime ActiveTime; 

    }
}
