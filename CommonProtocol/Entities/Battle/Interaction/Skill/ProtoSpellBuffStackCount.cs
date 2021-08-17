using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSpellBuffStackCount : BaseSkillProto
    {
        [Key(3)]
        public int Stack;
    }
}