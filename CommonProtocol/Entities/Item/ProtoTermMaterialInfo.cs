using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoTermMaterialInfo
    {
        [Key(1)]
        public  TermMaterialType TermMaterialType;

        [Key(2)]
        public int Count;

        [Key(3)]
        public long RecentUpdateDateTime;
    }
}
