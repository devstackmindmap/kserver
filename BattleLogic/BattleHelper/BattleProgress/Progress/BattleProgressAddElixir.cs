using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressAddElixir : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionAddElixir> _queueAddElixir = new ConcurrentQueue<BattleActionAddElixir>();

        public void Update()
        {
            if (_queueAddElixir.Count == 0)
                return;

            if (_queueAddElixir.TryDequeue(out var addElixir))
                addElixir.DoAction();
        }

        public bool IsProgress()
        {
            return _queueAddElixir.Count > 0;
        }

        public bool HasWork()
        {
            return IsProgress();
        }

        public void EnqueueAction<T>(T action)
        {
            _queueAddElixir.Enqueue(action as BattleActionAddElixir);
        }
    }
}