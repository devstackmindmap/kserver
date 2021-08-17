using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetBattleRecord : BaseProtocol
    {
        [Key(1)]
        public string RecordKey;
    }
}