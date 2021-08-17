using System.Collections.Generic;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleResult : BaseProtocol
    {
        [Key(1)]
        public List<ProtoBattleResultPlayerInfo> PlayerInfoList;

        [Key(2)]
        public BattleType BattleType;
    }
}
