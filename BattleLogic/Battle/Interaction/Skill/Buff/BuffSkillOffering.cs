using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_OFFERING)]
    public sealed class BuffSkillOffering : BuffSkill
    {
        private float _value2;

        public override float Value => _value2;
        public Unit Performer { get; private set; }

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _value2 = option.Value2;
            Performer = performer;
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value *= _value2;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillOffering();
        }
    }
}