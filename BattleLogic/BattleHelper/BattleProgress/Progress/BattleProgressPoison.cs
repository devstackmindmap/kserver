using AkaEnum.Battle;
using AkaUtility;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressPoison : IBattleProgress
    {
        private readonly Dictionary<PlayerType, UnitsPoison> _playersPoison = new Dictionary<PlayerType, UnitsPoison>(PlayerTypeComparer.Comparer);

        public BattleProgressPoison()
        {
            _playersPoison.Add(PlayerType.Player1, new UnitsPoison());
            _playersPoison.Add(PlayerType.Player2, new UnitsPoison());
        }

        public void Update()
        {
            foreach (var playerPoison in _playersPoison)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!playerPoison.Value.Poisons.ContainsKey(i))
                        continue;

                    if (playerPoison.Value.Poisons[i].IsPoison)
                    {
                        playerPoison.Value.Poisons[i].DecreaseHpByPoison();
                        playerPoison.Value.Remove(i);
                    }
                }
            }
        }

        public bool IsProgress()
        {
            return _playersPoison[PlayerType.Player1].IsProgress() || _playersPoison[PlayerType.Player2].IsProgress();
        }

        public void EnqueueAction<T>(T action)
        {
            var unitPoison = action as UnitPoison;
            _playersPoison[unitPoison.PlayerType].Enqueue(unitPoison);
        }

        public bool HasWork()
        {
            return IsProgress();
        }
    }

    public class UnitsPoison
    {
        private readonly ConcurrentDictionary<int, UnitPoison> _poisons = new ConcurrentDictionary<int, UnitPoison>();
        public ConcurrentDictionary<int, UnitPoison> Poisons => _poisons;

        public void Enqueue(UnitPoison unitPoison)
        {
            _poisons.TryAdd(unitPoison.UnitPositionIndex, unitPoison);
        }

        public bool IsProgress()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_poisons.ContainsKey(i))
                    continue;

                if (_poisons[i].IsPoison)
                {
                    return true;
                }
            }
            return false;
        }

        public void Remove(int unitPositionIndex)
        {
            _poisons.TryRemove(unitPositionIndex, out var unitPoison);
        }
    }
}