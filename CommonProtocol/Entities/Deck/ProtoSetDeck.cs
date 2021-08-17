using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSetDeck : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        //[Key(2)]
        //public ProtoTotalDeckInfo TotalDeckInfo;

        [Key(2)]
        public List<ProtoDeckElement> UpdateDecks;
    }
}
