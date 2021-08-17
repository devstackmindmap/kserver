using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoDeckElement
    {
        [Key(0)]
        public ModeType ModeType;

        [Key(1)]
        public SlotType SlotType;

        [Key(2)]
        public byte DeckNum;

        [Key(3)]
        public byte OrderNum;

        [Key(4)]
        public uint ClassId;
    }
}
