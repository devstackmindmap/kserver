
using AkaInterface;

namespace AkaData
{
    public class DataProbabilityGroup : ICorrection
    {
        public uint ProbabilityGroupId { get; set; }
        public uint ElementId { get; set; }
        public int Probability { get; set; }
        public int Correction { get; set; }
        public uint EventElementId { get; set; }

    }
}
