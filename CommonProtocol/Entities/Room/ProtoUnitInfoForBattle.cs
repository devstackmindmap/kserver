using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUnitInfoForBattle
    {
        [Key(0)]
        public uint Level;

        [Key(1)]
        public uint SkinId;

        [Key(2)]
        public ProtoWeaponInfoForBattle WeaponInfo;
    }
}
