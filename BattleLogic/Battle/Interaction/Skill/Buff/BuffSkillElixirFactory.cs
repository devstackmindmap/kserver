using AkaData;
using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_STATE_ELIXIR_FACTORY)]
    public class BuffSkillElixirFactory : BuffSkill
    {
        private float _value;

        public override float Value => _value;

        public override void DoSkill(DataSkillOption option, Unit performer, Unit target, int bulletTime)
        {
            base.DoSkill(option, performer, target, bulletTime);

            target.AddElixirFactory(option.Value2 * 1000, BuffStartTime);
        }

        public override IBuffSkill Clone()
        {
            return new BuffSkillElixirFactory();
        }

        public override void CalculateValue(ref float value, Unit target)
        {
        }
    }
}