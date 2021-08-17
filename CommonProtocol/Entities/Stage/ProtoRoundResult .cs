using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoRoundResult : BaseProtocol
    {
        [Key(1)]
        public uint StageLevelId;

        [Key(2)]
        public uint Round;
    }
}
