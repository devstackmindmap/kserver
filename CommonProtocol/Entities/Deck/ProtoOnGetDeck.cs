using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetDeck : BaseProtocol
    {
        [Key(1)]
        public List<ProtoDeckElement> DeckElements = new List<ProtoDeckElement>();
    }
}
