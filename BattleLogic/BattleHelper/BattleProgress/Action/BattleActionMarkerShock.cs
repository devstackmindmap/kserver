using AkaEnum;

namespace BattleLogic
{
    public sealed class BattleActionMarkerShock : BattleAction
    {
        private readonly Unit _target;
        private readonly float _value2;

        public BattleActionMarkerShock(Unit attacker, Unit target, float value2) : base(attacker)
        {
            _target = target;
            _value2 = value2;
        }

        public override BattleActionResult DoAction()
        {
            var damage = _target.UnitData.UnitStatus.MaxHp * _value2;
            var lastDamage = _target.DecreaseHp(_target, damage, DamageReasonType.SkillAttack, 0);
            S2CManager.SendMarkShock(_target, lastDamage);

            return new BattleActionResult() { IsDoing = true };
        }
    }
}