using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnSyncTime : BaseProtocol
    {
        [Key(1)]
        public long ServerTime;
    }
}