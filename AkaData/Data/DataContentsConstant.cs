using AkaEnum;
using AkaEnum.Battle;
using System.Collections.Generic;

namespace AkaData
{
    public class DataContentsConstant
    {
        public BattleType BattleType { get; set; }
        public ModeType DeckModeType { get; set; }
        public BattleEndConditionType EndConditionType { get; set; }
        public uint BattleTime { get; set; }
        public uint ExtensionTime { get; set; }
        public uint BoosterTime { get; set; }
        public uint CanRetreatTime { get; set; }
        public float ChargingElixirTime { get; set; }
        public float MaxElixir { get; set; }
        public float DefaultElixir { get; set; }
        public float BoosterElixirMultiple { get; set; }
        public bool CanRecordBattle { get; set; }
        public uint BattleRecordCount { get; set; }
        public uint BattleRecordLifeTime { get; set; }

        public uint UserExpWithWin { get; set; }
        public uint UserExpWithLose { get; set; }
        public uint UserExpWithDraw { get; set; }

        public uint BackgroundId { get; set; }

        public bool CanEmoticon { get; set; }

        public double BattleStartEffectTime { get; set; }

        public double BoostReduceShieldRate { get; set; }
    }
}
