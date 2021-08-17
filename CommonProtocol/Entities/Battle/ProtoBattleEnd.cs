using System.Collections.Generic;
using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleEnd : BaseProtocol
    {
        [Key(1)]
        public AkaEnum.Battle.PlayerType Winner;

        [Key(2)]
        public bool IsRetreat;

    }
}
