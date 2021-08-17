using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMatchingTryReEnterRoom : BaseProtocol
    {
        [Key(1)]
        public ProtoBattlePlayingInfo BattlePlayingInfo;
    }
}
