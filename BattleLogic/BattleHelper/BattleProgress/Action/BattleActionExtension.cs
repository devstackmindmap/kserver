using AkaEnum.Battle;
using System;

namespace BattleLogic
{
    public class BattleActionExtension : BattleAction
    {
        private Battle _battle;
        private DateTime _startExtensionDatetime;

        public BattleActionExtension(Battle battle) : base(null)
        {
            _battle = battle;
            _startExtensionDatetime = DateTime.UtcNow;
        }

        public override BattleActionResult DoAction()
        {
            _battle.StartBattleExtentionTime(_startExtensionDatetime);
            return new BattleActionResult() { IsDoing = false };
        }
    }
}