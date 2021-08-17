using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEventChallengeParam : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint ChallengeEventId;

        [Key(3)]
        public int DifficultLevel;
    }
}
