using System.Collections.Generic;

namespace AkaData
{
    public class DataStageLevelFlow
    {
        public uint StageLevelId { get; set; }
        public IList<uint> OpenStageIdList { get; set; }
    }
}
