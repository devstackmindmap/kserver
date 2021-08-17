using AkaEnum;
using AkaInterface;
using System.Collections.Generic;

namespace AkaData
{
    public class DataBackground : IProbability
    {
        public uint BackgroundId { get; set; }
        public uint BackgroundImageId { get; set; }
        public int Probability { get; set; }
    }
}
