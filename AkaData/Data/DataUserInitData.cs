using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataUserInitData
    {
        public uint Version { get; set; }
        public UserInitDataType UserInitDataType { get; set; }
        public uint TargetId { get; set; }
    }
}