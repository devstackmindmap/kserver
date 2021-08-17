using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnBattleResultList : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoOnBattleResult> BattleResult;
    }
}
