using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnServerState : BaseProtocol
    {

        [Key(1)]
        public int Sessions { get; set; }

    }
}
