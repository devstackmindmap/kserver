using System.Collections.Generic;

namespace AkaData
{
    public class DataProduct
    {
        public uint ProductId { get; set; }
        public IList<uint> RewardIdList { get; set; }
    }
}
