using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataDefaultDeckSet
    {
        public ModeType ModeType { get; set; }
        public List<uint> UnitIdList { get; set; }
        public List<uint> CardIdList { get; set; }
    }
}
