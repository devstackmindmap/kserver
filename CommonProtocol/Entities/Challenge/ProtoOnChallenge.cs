using AkaEnum;
using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnChallenge : BaseProtocol
    {
        [Key(1)]
        public ResultType ResultType;

        [Key(2)]
        public uint StageLevelId;

        [Key(3)]
        public uint StageRoundId;

        [Key(4)]
        public uint Round;
    }
}
