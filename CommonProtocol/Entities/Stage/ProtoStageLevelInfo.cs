using MessagePack;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoStageLevelInfo : BaseProtocol
    {
        [Key(1)]
        public uint StageLevelId;

        [Key(2)]
        public uint ClearCount;
    }
}
