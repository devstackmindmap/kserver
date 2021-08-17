using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoValidateCard : BaseProtocol
    {
        [Key(1)]
        public ValidateCardResultType ResultType;

        [Key(2)]
        public PlayerType PlayerType;

        [Key(3)]
        public double CurrentElixirCount;

        [Key(4)]
        public long RecentElixirChargingTime;

        [Key(5)]
        public int HandIndex;

        [Key(6)]
        public ProtoTargetUnitInfo PerformerUnitInfo;
    }
}