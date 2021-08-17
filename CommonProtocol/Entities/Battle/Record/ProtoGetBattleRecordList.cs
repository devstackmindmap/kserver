using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetBattleRecordList : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public List<BattleType> BattleTypeList;
    }
}