using System;
using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_SEVEN_DEVIL, SkillEffectType.BUFF_STATE_EIGHT_DEVIL)]
    public class BuffSkillSevenDevil : BuffSkill
    {
        private float _value2;
        private float _value3;

        public override float Value => _value3;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _value2 = option.Value2;
            _value3 = option.Value3;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillSevenDevil();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value = (float)Math.Round(value * _value2, 3);
        }
    }
}