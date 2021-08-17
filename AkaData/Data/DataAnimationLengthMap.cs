using System.Collections.Generic;

namespace AkaData
{
    public class DataAnimationLengthMap
    {
        public IDictionary<string, DataAnimationLengthMapValue> AnimationMap { get; set; }
    }

    public class DataAnimationLengthMapValue
    {
        public IDictionary<string, DataAnimationLength> AnimationLengths { get; set; }
    }

    public class DataAnimationLength
    {
        public int Bullet { get; set; }      // Milli
        public int TakeDamage { get; set; }  // Milli
    }
}
