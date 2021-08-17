using System.Collections.Generic;

namespace AkaData
{
    public class DataSkinGroup
    {
        public uint SkinGroupId { get; set; }
        public uint BaseSkinId { get; set; }
        public List<uint>SkinIdList { get; set; }
    }
}
