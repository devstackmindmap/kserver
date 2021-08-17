using AkaEnum;

namespace BattleLogic
{
    public class BattleActionElectricShock : BattleAction
    {
        private readonly Unit _target;
        private readonly float _value2;

        public BattleActionElectricShock(Unit attacker, Unit target, float value2) : base(attacker)
        {
            _target = target;
            _value2 = value2;
        }

        public override BattleActionResult DoAction()
        {
            var damage = BuffStateCalculator.GetAtkWithConditions(_target.UnitData.UnitStatus.Atk * _value2, _target, SkillEffectTypes.SkillAttackByAttackers, 0);
            var lastDamage = _target.DecreaseHp(Attacker, damage, DamageReasonType.ElectricShock, 0);
            S2CManager.SendElectricShock(_target, lastDamage);

            return new BattleActionResult() { IsDoing = true };
        }
    }
}