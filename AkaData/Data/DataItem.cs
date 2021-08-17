using AkaEnum;
using AkaInterface;

namespace AkaData
{
    public class DataItem : IProbability, IRandomCount
    {
        public uint ItemId { get; set; }
        public ItemType ItemType { get; set; }
        public uint ClassId { get; set; }
        public int MinNumber { get; set; }
        public int MaxNumber { get; set; }
        public int Probability { get; set; }
    }
}
