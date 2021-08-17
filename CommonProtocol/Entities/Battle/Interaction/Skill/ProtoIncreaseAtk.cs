using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoIncreaseAtk : BaseSkillProto
    {
        [Key(3)]
        public int AddAtk;
    }
}