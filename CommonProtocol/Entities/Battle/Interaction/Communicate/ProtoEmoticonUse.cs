using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEmoticonUse : BaseProtocol
    {
        [Key(1)]
        public string RoomId;
        
        [Key(2)]
        public PlayerType PlayerType;

        [Key(3)]
        public uint EmoticonId;

        [Key(4)]
        public bool IsPlayer;

    }
}