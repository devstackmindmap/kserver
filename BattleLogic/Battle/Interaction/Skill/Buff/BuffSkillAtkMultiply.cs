using System;
using AkaData;
using AkaEnum;
using Common;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_BERSERK, SkillEffectType.BUFF_STATE_WEAK, SkillEffectType.BUFF_STATE_WEAK_NORMAL, SkillEffectType.BUFF_STATE_WEAK_SKILL,
        SkillEffectType.BUFF_STATE_MADNESS, SkillEffectType.BUFF_STATE_REDUCTION)]
    class BuffSkillAtkMultiply : BuffSkill
    {
        private float _damageRate;

        public override float Value => _damageRate;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _damageRate = option.Value2;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillAtkMultiply();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value = (float)Math.Round(value * _damageRate, 3);
        }
    }
}
