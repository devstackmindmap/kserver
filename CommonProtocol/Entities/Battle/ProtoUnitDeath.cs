using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUnitDeath : BaseProtocol
    {
        [Key(1)]
        public uint UnitId;

        [Key(2)]
        public Dictionary<int, uint> HandCardStatIds;

        [Key(3)]
        public uint? NextCardStatId;
    }
}
