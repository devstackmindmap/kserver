using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRetreat : BaseProtocol
    {
        [Key(1)]
        public string RoomId;

        [Key(2)]
        public PlayerType PlayerType;
    }
}
