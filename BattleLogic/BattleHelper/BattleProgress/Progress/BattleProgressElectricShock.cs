using System.Collections.Concurrent;

namespace BattleLogic
{
    public class BattleProgressElectricShock : IBattleProgress
    {
        private readonly ConcurrentQueue<BattleActionElectricShock> _electricShocks = new ConcurrentQueue<BattleActionElectricShock>();

        public void Update()
        {
            if (_electricShocks.Count == 0)
                return;

            if (_electricShocks.TryDequeue(out var action))
                action?.DoAction();
        }

        public bool IsProgress()
        {
            return HasWork();
        }

        public bool HasWork()
        {
            return _electricShocks.Count > 0;
        }

        public void EnqueueAction<T>(T action)
        {
            _electricShocks.Enqueue(action as BattleActionElectricShock);
        }
    }
}