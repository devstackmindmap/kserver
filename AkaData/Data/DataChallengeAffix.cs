using System.Collections.Generic;
using AkaEnum;

namespace AkaData
{
    public class DataChallengeAffix
    {
        public uint Season { get; set; }
        public List<uint> NormalAffixIdList { get; set; }
        public List<uint> HardAffixIdList { get; set; }
    }
}
