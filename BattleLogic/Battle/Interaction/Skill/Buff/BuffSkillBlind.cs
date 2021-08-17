using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_BLIND)]
    public class BuffSkillBlind : BuffSkill
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
            return new BuffSkillBlind();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
            value *= _damageRate;
        }
    }
}