using AkaEnum;
using AkaUtility;

namespace BattleLogic
{
    public class BattleActionIgnitionBomb : BattleAction
    {
        private readonly Unit _target;
        private readonly float _value2;

        public BattleActionIgnitionBomb(Unit attacker, Unit target, float value2) : base(attacker)
        {
            _target = target;
            _value2 = value2;
        }

        public override BattleActionResult DoAction()
        {
            var atk = BuffStateCalculator.GetAtkWithConditions(Attacker.UnitData.UnitStatus.Atk, Attacker, SkillEffectTypes.SkillAttackByAttackers, 0);
            var ignitionBuff = _target.UnitBuffs.GetBuffSkill(SkillEffectType.BUFF_STATE_IGNITION);

            float damage = (ignitionBuff?.Value ?? 0) * _value2 * atk;
            var lastDamage = _target.DecreaseHp(Attacker, damage, DamageReasonType.SkillAttack, 0);
            S2CManager.SendIgnitionBomb(_target, lastDamage);
            _target.EnqueueBuffEnd(ignitionBuff);

            return new BattleActionResult() { IsDoing = true };
        }
    }
}