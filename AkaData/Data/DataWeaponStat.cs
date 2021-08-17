namespace AkaData
{
    public class DataWeaponStat
    {
        public uint WeaponStatId { get; set; }
        public uint WeaponId { get; set; }
        public uint Level { get; set; }
        public int RequirePieceCountForNextLevelUp { get; set; }
        public int NeedGoldForNextLevelUp { get; set; }
        public float ShieldDamageRate { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public float CriRate { get; set; }
        public float CriDmgRate { get; set; }
        public int AtkSpd { get; set; }
        public int Aggro { get; set; }

        public uint PassiveConditionId { get; set; }
    }
}
