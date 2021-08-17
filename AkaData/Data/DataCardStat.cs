using System.Collections.Generic;
using AkaEnum;

namespace AkaData
{
    public class DataCardStat
    {
        public uint CardStatId { get; set; }
        public uint CardId { get; set; }
        public uint Level { get; set; }
        public uint SkillConditionId { get; set; }
        public List<uint> SkillIdList { get; set; }
        public int Elixir { get; set; }
        public float HpCost { get; set; }
        public int RequirePieceCountForNextLevelUp { get; set; }
        public int NeedGoldForNextLevelUp { get; set; }
        public TargetGroupType TargetGroupType { get; set; }
        public TargetType TargetType { get; set; }
        public uint TargetId { get; set; }
    }
}
