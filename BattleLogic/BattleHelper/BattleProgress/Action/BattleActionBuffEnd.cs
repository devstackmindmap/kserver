namespace BattleLogic
{
    public class BattleActionBuffEnd : BattleAction
    {
        private readonly IBuffSkill _skillBuff;

        public BattleActionBuffEnd(Unit attacker, IBuffSkill skillBuff) : base(attacker)
        {
            _skillBuff = skillBuff;
        }

        public override BattleActionResult DoAction()
        {
            if (Attacker.RemoveConditionBuff(_skillBuff.SkillEffectType) == false)
                _skillBuff.BuffEnd();
            return new BattleActionResult() { IsDoing = true };
        }
    }
}
