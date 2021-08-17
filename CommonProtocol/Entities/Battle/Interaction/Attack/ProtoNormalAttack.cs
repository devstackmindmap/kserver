using System.Collections.Generic;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoNormalAttack : BaseProtocol
    {
        [Key(1)]
        public PlayerType PerformerPlayerType;

        [Key(2)]
        public uint PerformerUnitId;

        [Key(3)]
        public List<ProtoTargetUnitInfo> TargetUnitInfos;

        [Key(4)]
        public List<ProtoCounterTargetUnitInfo> CounterUnitInfos;

        [Key(5)]
        public long FinishTime;

        [Key(6)]
        public int GrowthAtk;

        [Key(7)]
        public List<ProtoNormalAttackAddBuffState> BuffStates;  // 다음공격으로 상대에게 버프를 걸때.
    }
}