using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBeforeBattleStart : BaseProtocol
    {
        [Key(1)]
        public PlayerType MyPlayerType;

        [Key(2)]
        public Dictionary<int, uint> HandCardStatIds;

        [Key(3)]
        public uint? NextCardStatId;

        [Key(4)]
        public string RoomId;

        [Key(5)]
        public ProtoPlayerInfo EnemyPlayer = new ProtoPlayerInfo();

        [Key(6)]
        public uint BackgroundImageId;
    }
}
