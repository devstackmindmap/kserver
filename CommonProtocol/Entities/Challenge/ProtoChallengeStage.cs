using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoChallengeStage
    {
        [Key(0)]
        public int Day;

        [Key(1)]
        public int DifficultLevel;

        [Key(2)]
        public int ClearCount;

        [Key(3)]
        public bool IsRewarded;

        [Key(4)]
        public int RewardResetCount;
    }
}
