using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCurrentUnitInfo
    {
        [Key(0)]
        public int UnitPositionIndex;

        [Key(1)]
        public uint UnitId;

        [Key(2)]
        public uint Level;

        [Key(3)]
        public int Hp;

        [Key(4)]
        public int GrowthAtk;

        [Key(5)]
        public List<ProtoShieldInfo> Shields;

        [Key(6)]
        public List<ProtoCurrentBuffInfo> Buffs;

        [Key(7)]
        public long NextAttackTime;

        [Key(8)]
        public float GrowthCriticalRate;

        [Key(9)]
        public float GrowthCriticalDamageRate;

        [Key(10)]
        public uint SkinId;

        [Key(11)]
        public ProtoEquipInfo WeaponInfo;

        [Key(12)]
        public float AttackSpeed;
    }
}
