using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetBattleRecordList : BaseProtocol
    {
        [Key(1)]
        public List<ProtoBattleRecord> BattleRecordList;
    }
}