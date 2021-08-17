using AkaEnum.Battle;
using CommonProtocol.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleRecordPlayer
    {
        [Key(0)]
        public int Score;

        [Key(1)]
        public uint UserId;

        [Key(2)]
        public string NickName;
        
        [Key(3)]
        public int TeamRankPoint;

        [Key(4)]
        public int UserRankPoint;

        [Key(5)]
        public int GettingRankPoint;
        
        [Key(6)]
        public List<ProtoUnitInfo> Units;

        [Key(7)]
        public List<uint> CardStatIds;
    }

}