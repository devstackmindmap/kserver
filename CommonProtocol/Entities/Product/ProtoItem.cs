using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoItem
    {
        [Key(0)]
        public ItemType ItemType;

        [Key(1)]
        public uint ClassId;

        [Key(2)]
        public int MinNumber;

        [Key(3)]
        public int MaxNumber;
    }
}

