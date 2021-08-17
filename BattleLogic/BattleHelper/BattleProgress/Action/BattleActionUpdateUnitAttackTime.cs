using AkaEnum.Battle;
using CommonProtocol;
using CommonProtocol.Battle;
using System;

namespace BattleLogic
{
    public class BattleActionUpdateUnitAttackTime : BattleAction
    {
        private readonly float _attackSpeed;

        public BattleActionUpdateUnitAttackTime(Unit attacker, float attackSpeed) : base(attacker)
        {
            _attackSpeed = attackSpeed;
        }

        public override BattleActionResult DoAction()
        {
            S2CManager.SendUpdateUnitAttackTime(Attacker, _attackSpeed);

            return new BattleActionResult() { IsDoing = true };
        }
    }
}