using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnEmoticonUse : BaseProtocol
    {
        [Key(1)]
        public PlayerType PlayerType;

        [Key(2)]
        public uint EmoticonId;

        [Key(3)]
        public bool IsPlayer;
    }
}