using AkaEnum.Battle;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleResultChallenge : ProtoBattleResult
    {
        [Key(3)]
        public uint Season;

        [Key(4)]
        public int Day;

        [Key(5)]
        public int DifficultLevel;
    }
}
