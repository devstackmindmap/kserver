using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleStart : BaseProtocol
    {
        [Key(1)]
        public long BattleStartTime;
    }
}
