using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEquipInfo
    {
        [Key(0)]
        public uint Id;

        [Key(1)]
        public uint Level;
    }
}