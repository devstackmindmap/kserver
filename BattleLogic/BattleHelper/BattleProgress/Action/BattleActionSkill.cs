using System;
using CommonProtocol;

namespace BattleLogic
{
    public class BattleActionSkill : BattleAction
    {
        private readonly CardUseActionData _data;

        public BattleActionSkill(Unit attacker, CardUseActionData data) : base(attacker)
        {
            _data = data;
        }

        public override BattleActionResult DoAction()
        {
            var result = Attacker.DoSkill(_data);
            if (result.IsDoing)
                Attacker.BattleHelper.BattlePatternBehavior.DoSkillPatternSchedule(Attacker.PlayerType, Attacker.UnitData.UnitIdentifier.UnitId, _data.UseCard);
            return result;
        }
    }
}
