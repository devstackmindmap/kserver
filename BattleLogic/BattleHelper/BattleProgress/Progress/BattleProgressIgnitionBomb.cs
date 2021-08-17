using System.Collections.Concurrent;

namespace BattleLogic
{
    public class BattleProgressIgnitionBomb : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionIgnitionBomb> _ignitionBombs = new ConcurrentQueue<BattleActionIgnitionBomb>();

        public void Update()
        {
            if (_ignitionBombs.Count == 0)
                return;

            if (_ignitionBombs.TryDequeue(out var action))
                action?.DoAction();
        }

        public bool IsProgress()
        {
            return HasWork();
        }

        public bool HasWork()
        {
            return _ignitionBombs.Count > 0;
        }

        public void EnqueueAction<T>(T action)
        {
            _ignitionBombs.Enqueue(action as BattleActionIgnitionBomb);
        }
    }
}