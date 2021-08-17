using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoSystemMailData : ProtoMailData
    {
        [Key(4)]
        public uint SystemMailId;
    }
}
