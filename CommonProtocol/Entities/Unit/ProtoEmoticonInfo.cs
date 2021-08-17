using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEmoticonInfo
    {
        [Key(0)]
        public uint EmoticonId;

        [Key(1)]
        public uint UnitId;

        [Key(2)]
        public int OrderNum;

    }
}
