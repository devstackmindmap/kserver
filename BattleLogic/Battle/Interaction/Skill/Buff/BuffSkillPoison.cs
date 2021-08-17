using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_POISON)]
    public class BuffSkillPoison : BuffSkill
    {
        private Unit _target;

        public override float Value { get; }

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            _target = target;
            SkillEffectType = option.SkillEffectType;
            target.AddPoison(option.Value3);
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillPoison();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }

        public override void BuffEnd()
        {
            _target.PoisonEnd();
        }
    }
}