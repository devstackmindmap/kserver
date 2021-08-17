using System.Collections.Generic;

namespace AkaData
{
    public class DataAnimationEvent
    {
        public uint AnimationEventId { get; set; }
        public int Length { get; set; } //1 / 100 단위

        public List<int> AttackTimingList { get; set; }
    }
}
