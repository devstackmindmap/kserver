using AkaEnum.Battle;
using System.Collections.Generic;

namespace AkaData
{
    public class DataStageRound
    {
        public uint StageRoundId { get; set; }
        public uint StageLevelId { get; set; }
        public uint Round { get; set; }
        public List<uint> MonsterGroupIdList { get; set; }
        public string StartDialogFileName { get; set; }
        public string EndDialogFileName { get; set; }
        public uint BackgroundImageId { get; set; }
        public BattleEndConditionType EndConditionType { get; set; }
    }
}
