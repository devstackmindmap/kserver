using AkaEnum;
using System.Collections.Generic;

namespace AkaData
{
    public class DataSquareObject
    {
        public uint SquareObjectId { get; set; }
        public uint SquareObjectLevel { get; set; }
        public int MaxShield { get; set; }
        public int NeedExpForNextLevelUp { get; set; }
        public int NeedGoldForNextLevelUp { get; set; }
        public IList<IList<uint>> InvasionLvLists { get; set; }
    }
}