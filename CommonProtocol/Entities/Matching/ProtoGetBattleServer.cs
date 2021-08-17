using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoGetBattleServer : BaseProtocol
    {
        [Key(1)]
        public int GroupCode;
    }
}
