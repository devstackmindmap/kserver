using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUintAndUintList : BaseProtocol
    {
        [Key(1)]
        public uint Num;

        [Key(2)]
        public List<uint> Items = new List<uint>();
    }
}
