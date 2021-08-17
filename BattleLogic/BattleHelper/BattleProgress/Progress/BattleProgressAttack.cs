using System;
using System.Collections.Concurrent;

namespace BattleLogic
{
    public class BattleProgressAttack : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionAttack> _attacks = new ConcurrentQueue<BattleActionAttack>();        
        private BattleActionAttackResult _result;

        public void Update()
        {
            if (_result?.IsDoing == true)
            {
                CheckFinishTime();
                return;
            }

            if (_attacks.Count == 0)
                return;

            _result = null;
            if ( _attacks.TryDequeue(out var attack))
                _result = attack.DoAction() as BattleActionAttackResult;
        }

        public bool IsProgress()
        {
            return _result?.IsDoing ?? false;
        }

        public bool HasWork()
        {
            return _attacks.Count > 0;
        }

        public void EnqueueAction<T>(T action)
        {
            _attacks.Enqueue(action as BattleActionAttack);     
        }

        private void CheckFinishTime()
        {
            if (DateTime.UtcNow < _result.FinishTime)
                return;

            _result.Performer.AttackTimer?.Start();
            _result.Performer.UnitActionStatus = UnitActionStatus.None;
            _result.IsDoing = false;
        }
    }
}