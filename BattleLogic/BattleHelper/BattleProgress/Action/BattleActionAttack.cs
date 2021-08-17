namespace BattleLogic
{
    public class BattleActionAttack : BattleAction
    {
        int _attackCount;

        public BattleActionAttack(Unit attacker, int attackCount) : base(attacker)
        {
            _attackCount = attackCount;
        }

        public override BattleActionResult DoAction()
        {
            var result = Attacker.DoAttack();
            if (result.IsDoing)
            {
                Attacker.BattleHelper.BattlePatternBehavior.DoAttackPatternSchedule(Attacker.PlayerType, Attacker.UnitData.UnitIdentifier.UnitId);
            }

            _attackCount--;
            if (_attackCount > 0)
            {
                Attacker.BattleHelper.EnqueueSequantialAttack(new BattleActionAttack(Attacker, _attackCount));
            }
            return result;
        }
    }
}
