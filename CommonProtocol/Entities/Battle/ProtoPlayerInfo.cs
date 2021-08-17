using CommonProtocol.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoPlayerInfo
    {
        [Key(0)]
        public string Nickname;

        [Key(1)]
        public uint UserId;

        [Key(2)]
        public List<ProtoUnitInfo> Units = new List<ProtoUnitInfo>();

        [Key(3)]
        public List<uint> CardStatIds;
        
        [Key(4)]
        public uint ProfileIconId;
    }
}
