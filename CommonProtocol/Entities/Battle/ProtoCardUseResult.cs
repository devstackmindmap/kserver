using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoCardUseResult : BaseProtocol
    {
        [Key(1)]
        public int HandIndex;

        [Key(2)]
        public ElixirCountState ElixirCountState;
    }
}
