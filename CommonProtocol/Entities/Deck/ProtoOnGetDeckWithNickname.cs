using AkaEnum;
using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetDeckWithNickname : BaseProtocol
    {
        [Key(1)]
        public string Nickname;

        [Key(2)]
        public ProtoOnGetRecentDeck DeckElements;
    }
}
