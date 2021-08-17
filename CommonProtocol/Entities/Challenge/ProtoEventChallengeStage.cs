using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoEventChallengeStage
    {
        [Key(0)]
        public int DifficultLevel;

        [Key(1)]
        public int ClearCount;

        [Key(2)]
        public bool IsRewarded;

        [Key(3)]
        public int RewardResetCount;
    }
}
