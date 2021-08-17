using AkaEnum;

namespace AkaData
{
    public class DataSkill
    {
        public uint SkillId { get; set; }
        public SkillType SkillType { get; set; }
        public uint[] SkillOptionList { get; set; }
        public AnimationType AnimationType { get; set; }
        public string UnitInitial { get; set; }
    }
}
