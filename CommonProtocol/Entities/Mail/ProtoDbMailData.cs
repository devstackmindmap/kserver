using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoDbMailData : ProtoMailData
    {
        [Key(4)]
        public string MailIcon;

        [Key(5)]
        public uint ProductId;

        [Key(6)]
        public string MailTitle;

        [Key(7)]
        public string MailText;
    }
}
