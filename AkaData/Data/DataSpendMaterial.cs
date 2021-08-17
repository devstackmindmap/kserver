using AkaEnum;

namespace AkaData
{
    public class DataSpendMaterial
    {
        public int Order { get; set; }
        public MaterialType MaterialType { get; set; }
        public int Value { get; set; }
        public BehaviorType BehaviorType { get; set;}
    }
}