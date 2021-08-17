using System.Collections.Concurrent;

namespace BattleLogic
{
    public sealed class BattleProgressMarkerShock : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionMarkerShock> _markShocks = new ConcurrentQueue<BattleActionMarkerShock>();

        public void Update()
        {
            if (_markShocks.Count == 0)
                return;

            if (_markShocks.TryDequeue(out var action))
                action?.DoAction();
        }

        public bool IsProgress()
        {
            return HasWork();
        }

        public bool HasWork()
        {
            return _markShocks.Count > 0;
        }

        public void EnqueueAction<T>(T action)
        {
            _markShocks.Enqueue(action as BattleActionMarkerShock);
        }
    }
}