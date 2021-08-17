using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnLevelUp : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public int Gold;

        [Key(3)]
        public List<uint> NewCardIds;
    }
}