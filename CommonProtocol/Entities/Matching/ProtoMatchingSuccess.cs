using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMatchingSuccess : BaseProtocol
    {
        [Key(1)]
        public string RoomId;
        [Key(2)]
        public uint StageRoundId;

        [Key(3)]
        public string BattleServerIp;

        [Key(4)]
        public string BattleServerPort;

        [Key(5)]
        public int EnemyRankPoint;

        [Key(6)]
        public int EnemyUserRankPoint;

        [Key(7)]
        public BattleType BattleType;

        [Key(8)]
        public uint MyWins;

        [Key(9)]
        public uint EnemyWins;
    }
}
