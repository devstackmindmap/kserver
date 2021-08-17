using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoChallenge : ProtoChallengeParam
    {
        [Key(5)]
        public bool IsStart;
    }
}
