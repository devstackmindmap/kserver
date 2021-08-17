using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSpellShield : BaseSkillProto
    {
        [Key(3)]
        public int Shield;

        [Key(4)]
        public long StartTime;

        [Key(5)]
        public long EndTime;

        [Key(6)]
        public uint CardId;
    }
}