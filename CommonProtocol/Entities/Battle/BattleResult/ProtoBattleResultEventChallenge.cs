using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleResultEventChallenge : ProtoBattleResult
    {
        [Key(3)]
        public uint ChallengeEventId;

        [Key(4)]
        public int DifficultLevel;
    }
}
