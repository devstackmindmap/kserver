using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressBuffEnd : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionBuffEnd> _queueBuffEnd = new ConcurrentQueue<BattleActionBuffEnd>();

        public void Update()
        {
            if (_queueBuffEnd.Count == 0)
                return;

            if (_queueBuffEnd.TryDequeue(out var buffEnd))
                buffEnd.DoAction();
        }

        public bool IsProgress()
        {
            return _queueBuffEnd.Count > 0;
        }

        public bool HasWork()
        {
            return IsProgress();
        }

        public void EnqueueAction<T>(T action)
        {
            _queueBuffEnd.Enqueue(action as BattleActionBuffEnd);
        }
    }
}