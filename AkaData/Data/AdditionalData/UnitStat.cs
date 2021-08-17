using System.Collections.Generic;

namespace AkaData
{
    public class UnitStat
    {
        public uint UnitId;
        public uint Level;
        public int RequirePieceCountForNextLevelUp;
        public int AtkSpd;
        public int Atk;
        public int Hp;
        public float CriRate;
        public float CriDmgRate;
        public int Aggro;
        public int NeedGoldForNextLevelUp;
        public List<uint> PassiveConditionId;
    }
}
