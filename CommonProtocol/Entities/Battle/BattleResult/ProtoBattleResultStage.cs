using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleResultStage : ProtoBattleResult
    {
        [Key(3)]
        public uint StageLevelId;
    }
}
