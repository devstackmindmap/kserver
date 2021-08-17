using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoInvalidateCardUse : BaseProtocol
    {
        [Key(1)]
        public uint ReplacedCardStatId;

        [Key(2)]
        public int ReplacedHandIndex;

        [Key(3)]
        public uint? NextCardStatId;
    }
}