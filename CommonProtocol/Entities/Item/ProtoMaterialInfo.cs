using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMaterialInfo
    {
        [Key(1)]
        public MaterialType MaterialType;

        [Key(2)]
        public int Count;
    }
}
