using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoMailData
    {
        [Key(0)]
        public uint MailId;

        [Key(1)]
        public long StartDateTime;

        [Key(2)]
        public long EndDateTime;

        [Key(3)]
        public bool IsRead;
    }
}
