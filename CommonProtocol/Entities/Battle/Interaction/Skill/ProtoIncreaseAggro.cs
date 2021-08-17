using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public sealed class ProtoIncreaseAggro : BaseSkillProto
    {
        [Key(3)]
        public int AddAggro;
    }
}