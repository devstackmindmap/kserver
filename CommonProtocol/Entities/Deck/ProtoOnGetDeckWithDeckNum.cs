using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetDeckWithDeckNum : BaseProtocol
    {
        [Key(1)]
        public Dictionary<uint, ProtoDeckWithDeckNum> UserAndDecks;
    }
}
