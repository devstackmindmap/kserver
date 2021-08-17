using AkaEnum.Battle;
using System;

namespace BattleLogic
{
    public class BattleActionEnd : BattleAction
    {
        private Action<PlayerType, bool> _battleEnd;
        private readonly PlayerType _winner;
        private readonly bool _isRetreat;

        public BattleActionEnd(Action<PlayerType, bool> battleEnd, PlayerType winner, bool isRetreat) : base(null)
        {
            _battleEnd = battleEnd;
            _winner = winner;
            _isRetreat = isRetreat;
        }

        public override BattleActionResult DoAction()
        {
            _battleEnd(_winner, _isRetreat);
            _battleEnd = null;

            AkaLogger.Logger.Instance().Debug($"BattleActionEnd DidAction");
            return new BattleActionResult() { IsDoing = true };
        }
    }
}
