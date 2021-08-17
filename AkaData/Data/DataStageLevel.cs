using AkaEnum;
using AkaEnum.Battle;
using AkaInterface;

namespace AkaData
{
    public class DataStageLevel : IBox
    {
        public uint StageLevelId { get; set; }
        public uint RoguelikeSaveDeckId { get; set; }
        public uint RewardId { get; set; }
        public StageType StageType { get; set; }     
        public bool IsAutoOpen { get; set; }
    }
}
