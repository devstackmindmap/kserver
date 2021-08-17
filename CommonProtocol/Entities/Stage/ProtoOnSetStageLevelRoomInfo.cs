using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSetStageLevelRoomInfo : BaseProtocol
    {
        [Key(1)]
        public bool Result;
    }
}
