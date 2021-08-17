using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCurrentPlayerInfo
    {
        [Key(0)]
        public string Nickname;

        [Key(1)]
        public uint UserId;

        [Key(2)]
        public List<ProtoCurrentUnitInfo> Units = new List<ProtoCurrentUnitInfo>();

        [Key(3)]
        public List<uint> CardStatIds = new List<uint>();

        [Key(4)]
        public uint ProfileIconId;
    }
}
