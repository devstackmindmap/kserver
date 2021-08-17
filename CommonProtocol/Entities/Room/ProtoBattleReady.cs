using MessagePack;
using AkaEnum;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleReady : BaseProtocol
    {
        [Key(1)]
        public string RoomId;
        
        [Key(2)]
        public uint UserId;

    }
}
