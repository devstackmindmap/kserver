using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_TARGET)]
    public sealed class BuffSkillTarget : BuffSkill
    {
        private float _value2;

        public override float Value { get; }

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _value2 = option.Value2;
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            var targetBuff = target.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_TARGET);
            if (targetBuff == null || targetBuff.IsValid() == false)
                return;

            value *= _value2;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillTarget();
        }
    }
}