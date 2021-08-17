using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSetCurrentHp : BaseSkillProto
    {
        [Key(3)]
        public int CurrentHp;
    }
}