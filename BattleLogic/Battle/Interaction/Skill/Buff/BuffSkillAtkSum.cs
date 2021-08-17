using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_BARISADA)]
    public class BuffSkillAtkSum : BuffSkill
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
            return new BuffSkillAtkSum();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value += _value;
        }
    }
}