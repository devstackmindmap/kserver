using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_GROWTH_ATK)]
    public class BuffSkillGrowthAtk : BuffSkill
    {
        private float _value;

        public override float Value => _value;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);
            _value = option.Value2;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillGrowthAtk();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }
    }
}