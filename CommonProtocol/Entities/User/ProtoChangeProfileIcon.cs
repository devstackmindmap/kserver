using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoChangeProfileIcon : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public ProfileIconType ProfileIconType;

        [Key(3)]
        public uint ProfileIconId;
    }
}
