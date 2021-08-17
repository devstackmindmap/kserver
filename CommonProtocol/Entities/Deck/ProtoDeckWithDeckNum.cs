using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoDeckWithDeckNum : BaseProtocol
    {
        [Key(1)]
        public ProtoOnGetDeck Deck;
        
        [Key(2)]
        public string Nickname;

        [Key(3)]
        public Dictionary<int, ProtoUnitInfoForBattle> UnitsInfo = new Dictionary<int, ProtoUnitInfoForBattle>();

        [Key(4)]
        public Dictionary<int, uint> CardsLevel = new Dictionary<int, uint>();

        [Key(5)]
        public byte DeckNum;

        [Key(6)]
        public uint ProfileIconId;
    }
}
