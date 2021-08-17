using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_SHIELD_FREEZE, SkillEffectType.BUFF_STATE_IMMORTAL, SkillEffectType.BUFF_STATE_IMMUNE, SkillEffectType.BUFF_STATE_DOUBLE_ATTACK,
        SkillEffectType.BUFF_STATE_TAUNT, SkillEffectType.BUFF_STATE_BUFF_IMMUNE, SkillEffectType.BUFF_STATE_ATTENTION, SkillEffectType.BUFF_STATE_STEALTH, SkillEffectType.BUFF_STATE_IMMUNE_ELECTRIC_DISAPPEARANCE,
        SkillEffectType.BUFF_STATE_IMMUNE_DISAPPEARANCE, SkillEffectType.BUFF_STATE_SILENCE)]
    public class BuffSkillEmpty : BuffSkill
    {
        private float _value2;

        public override float Value => _value2;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            _value2 = option.Value2;
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillEmpty();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }
    }
}