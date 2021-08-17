using MessagePack;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoOnStageLevelExit : BaseProtocol
    {
        [Key(1)]
        public List<uint> OpenedStageLevelIdList;

        [Key(2)]
        public uint StageLevelId;

        [Key(3)]
        public bool IsWin;

        [Key(4)]
        public uint ClearCount;
    }
}
