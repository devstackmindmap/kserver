using System.Collections.Generic;
using AkaEnum;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSkill : BaseProtocol
    {
        [Key(1)]
        public PlayerType PerformerPlayerType;

        [Key(2)]
        public uint PerformerUnitId;

        [Key(3)]
        public List<ProtoSkillOptionResult> SkillOptionResults;

        [Key(4)]
        public uint UsedCardStatId;

        [Key(5)]
        public long BulletTimeStart;

        [Key(6)]
        public uint SkillId;

        [Key(7)]
        public List<ProtoTargetUnitInfo> CounterUnitInfos;

        [Key(8)]
        public AnimationType CounterAnimationType;

        [Key(9)]
        public uint ReplacedCardStatId;

        [Key(10)]
        public int ReplacedHandIndex;

        [Key(11)]
        public uint? NextCardStatId;
    }
}