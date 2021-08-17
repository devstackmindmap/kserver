using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoShieldInfo
    {
        [Key(0)]
        public uint CardId;

        [Key(1)]
        public long StartTime;

        [Key(2)]
        public long EndTime;

        [Key(3)]
        public int Shield;
    }
}