using System.Collections.Generic;
using AkaEnum;
using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleResultPlayerInfo : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public byte DeckNum;
        
        [Key(3)]
        public BattleResultType BattleResultType;

        [Key(4)]
        public List<ProtoActionStatus> ActionStatusLog;
    }
}
