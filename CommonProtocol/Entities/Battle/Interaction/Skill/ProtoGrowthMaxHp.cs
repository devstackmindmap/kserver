using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGrowthMaxHp : BaseSkillProto
    {
        [Key(3)]
        public int GrowthMaxHp;
    }
}