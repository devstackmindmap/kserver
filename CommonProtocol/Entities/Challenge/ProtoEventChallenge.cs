using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEventChallenge : ProtoEventChallengeParam
    {
        [Key(4)]
        public bool IsStart;
    }
}
