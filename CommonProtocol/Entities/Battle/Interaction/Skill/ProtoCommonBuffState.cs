using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCommonBuffState : BaseSkillProto
    {
        [Key(3)]
        public long StartTime;

        [Key(4)]
        public long EndTime;
    }
}