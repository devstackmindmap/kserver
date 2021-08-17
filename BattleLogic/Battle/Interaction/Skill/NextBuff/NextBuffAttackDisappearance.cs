using AkaEnum;

namespace BattleLogic
{
    [Skill(SkillEffectType.BUFF_NEXT_ATTACK_DISAPPEARANCE)]
    public class NextBuffAttackDisappearance : NextBuff
    {
        public override void CalculateValue(ref float value, Unit target, Unit performer, double delay)
        {
        }

        public override INextBuff Clone()
        {
            return new NextBuffAttackDisappearance();
        }
    }
}