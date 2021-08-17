using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCurrentBattleStatus : BaseProtocol
    {
        [Key(1)]
        public long BattleStartTime;

        [Key(2)]
        public PlayerType MyPlayerType;

        [Key(3)]
        public Dictionary<int, uint> HandCardStatIds;

        [Key(4)]
        public Dictionary<int, int> HandCardDecreaseElixir;

        [Key(5)]
        public uint? NextCardStatId;

        [Key(6)]
        public List<ProtoCurrentUnitInfo> Units = new List<ProtoCurrentUnitInfo>();

        [Key(7)]
        public ProtoCurrentPlayerInfo EnemyPlayer = new ProtoCurrentPlayerInfo();

        [Key(8)]
        public int AccumulatedBulletTime;

        [Key(9)]
        public BattleType BattleType;

        [Key(10)]
        public uint BackgroundImageId;

        [Key(11)]
        public uint StageRoundId;


        //TODO treasure info
    }
}
