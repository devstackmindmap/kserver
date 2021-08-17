using AkaEnum;

namespace AkaData
{
    public class DataSkillOption
    {
        public uint SkillOptionId { get; set; }
        public float Value1 { get; set; }
        public float Value2 { get; set; }
        public int Value3 { get; set; }
        public float Value4 { get; set; }
        public AnimationType AnimationType { get; set; }
        public SkillEffectType SkillEffectType { get; set; }
        public SkillEffectGroupType SkillEffectGroupType { get; set; }
        public SkillEffectTimeType SkillEffectTimeType { get; set; }
        public TargetGroupType TargetGroupType { get; set; }
        public TargetType TargetType { get; set; }
        public uint TargetId { get; set; }
    }
}
