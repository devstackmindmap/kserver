using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressCardUse : IBattleProgress
    {

        private readonly ConcurrentQueue<BattleActionCardUse> _cardUses = new ConcurrentQueue<BattleActionCardUse>();

        public void Update()
        {
            if (_cardUses.TryDequeue(out var actionCardUse))
            {
                actionCardUse.DoAction();
            }
        }

        public bool IsProgress()
        {
            return HasWork();
        }

        public bool HasWork()
        {
            return _cardUses.IsEmpty == false;
        }

        public void EnqueueAction<T>(T action)
        {
            _cardUses.Enqueue(action as BattleActionCardUse);
        }
    }
}