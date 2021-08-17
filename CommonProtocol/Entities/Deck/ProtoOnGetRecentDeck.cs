using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnGetRecentDeck : BaseProtocol
    {
        [Key(1)]
        public Dictionary<int, ProtoUnitInfoForRecentDeck> UnitsInfo = new Dictionary<int, ProtoUnitInfoForRecentDeck>();

        [Key(2)]
        public Dictionary<int, ProtoCardInfoExceptCount> CardsInfo = new Dictionary<int, ProtoCardInfoExceptCount>();
    }
}
