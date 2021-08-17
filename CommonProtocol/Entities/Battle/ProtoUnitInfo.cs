using MessagePack;

namespace CommonProtocol.Battle
{
    [MessagePackObject]
    public class ProtoUnitInfo
    {
        [Key(0)]
        public uint UnitId;

        [Key(1)]
        public uint Level;

        [Key(2)]
        public uint SkinId;

        [Key(3)]
        public ProtoEquipInfo WeaponInfo;

        [Key(4)]
        public bool IsDeath;
    }
}
