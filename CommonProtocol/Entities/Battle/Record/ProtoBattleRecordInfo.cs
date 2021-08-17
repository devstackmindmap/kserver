using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleRecordInfo
    {

        [Key(0)]
        public PlayerType Winner;
        
        [Key(1)]
        public Dictionary<PlayerType, ProtoBattleRecordPlayer> BattleStartInfo;
    }
}