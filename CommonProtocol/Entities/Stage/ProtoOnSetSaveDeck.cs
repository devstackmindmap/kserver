using AkaEnum.Battle;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSetSaveDeck : BaseProtocol
    {
        [Key(1)]
        public bool Result;
    }
}
