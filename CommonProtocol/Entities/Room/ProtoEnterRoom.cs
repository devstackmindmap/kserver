using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEnterRoom : BaseProtocol
    {
        [Key(1)]
        public string RoomId;
        
        [Key(2)]
        public uint UserId;

        [Key(3)]
        public byte DeckNum;

        [Key(4)]
        public AkaEnum.Battle.BattleType BattleType;

        [Key(5)]
        public string BattleServerIp;


    }
}
