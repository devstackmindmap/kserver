using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoItemResult
    {
        [Key(0)]
        public ItemType ItemType;

        [Key(1)]
        public uint ClassId;

        [Key(2)]
        public int Count;
    }
}
