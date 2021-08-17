using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressUpdateUnitAttackTime : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionUpdateUnitAttackTime> _actions = new ConcurrentQueue<BattleActionUpdateUnitAttackTime>();

        public void Update()
        {
            if (_actions.Count == 0)
                return;

            if (_actions.TryDequeue(out var action))
                action.DoAction();
        }

        public bool IsProgress()
        {
           // return false;
            return _actions.Count > 0;
        }


        public bool HasWork()
        {
            return _actions.Count > 0;
        }

        public void EnqueueAction<T>(T action)
        {
            _actions.Enqueue(action as BattleActionUpdateUnitAttackTime);
        }
    }
}