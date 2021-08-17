using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoUserProfileInfo
    {
        [Key(0)]
        public uint UserProfileId;
    }
}
