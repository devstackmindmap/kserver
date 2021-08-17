using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoChallengeParam : BaseProtocol
    {
        [Key(1)]
        public uint UserId;

        [Key(2)]
        public uint Season;

        [Key(3)]
        public int Day;

        [Key(4)]
        public int DifficultLevel;
    }
}
