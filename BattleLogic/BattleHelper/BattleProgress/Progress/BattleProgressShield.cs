using AkaEnum.Battle;
using AkaUtility;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace BattleLogic
{
    public class BattleProgressShield : IBattleProgress
    {
        private readonly Dictionary<PlayerType, UnitsSheild> _playersShield = new Dictionary<PlayerType, UnitsSheild>(PlayerTypeComparer.Comparer);

        public BattleProgressShield()
        {
            _playersShield.Add(PlayerType.Player1, new UnitsSheild());
            _playersShield.Add(PlayerType.Player2, new UnitsSheild());
        }

        public void Update()
        {
            foreach (var playerShield in _playersShield)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!playerShield.Value.Shields.ContainsKey(i))
                        continue;

                    if (playerShield.Value.Shields[i].IsShield)
                    {
                        playerShield.Value.Shields[i].DecreaseShield();
                        playerShield.Value.Remove(i);
                    }
                }
            }
        }

        public bool IsProgress()
        {
            return _playersShield[PlayerType.Player1].IsProgress() || _playersShield[PlayerType.Player2].IsProgress();
        }
        public bool HasWork()
        {
            return IsProgress();
        }

        public void EnqueueAction<T>(T action)
        {
            var unitSheild = action as UnitShieldOld;
            _playersShield[unitSheild.PlayerType].Enqueue(unitSheild);
        }
    }

    public class UnitsSheild
    {
        private readonly ConcurrentDictionary<int, UnitShieldOld> _shields = new ConcurrentDictionary<int, UnitShieldOld>();
        public ConcurrentDictionary<int, UnitShieldOld> Shields => _shields;

        public void Enqueue(UnitShieldOld unitShield)
        {
            _shields.TryAdd(unitShield.UnitPositionIndex, unitShield);
        }

        public bool IsProgress()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_shields.ContainsKey(i))
                    continue;

                if (_shields[i].IsShield)
                {
                    return true;
                }
            }
            return false;
        }

        public void Remove(int unitPositionIndex)
        {
            _shields.TryRemove(unitPositionIndex, out var unitShield);
        }
    }
}