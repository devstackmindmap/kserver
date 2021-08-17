using System.Collections.Generic;

namespace AkaData
{
    public class DataUnitStat
    {
        public uint UnitStatId { get; set; }
        public uint UnitId { get; set; }
        public uint Level { get; set; }
        public int RequirePieceCountForNextLevelUp { get; set; }
        public int AtkSpd { get; set; }
        public int Atk { get; set; }
        public int Hp { get; set; }
        public float CriRate { get; set; }
        public float CriDmgRate { get; set; }
        public int Aggro { get; set; }
        public int NeedGoldForNextLevelUp { get; set; }
        public List<uint> PassiveConditionId { get; set; }
    }
}
