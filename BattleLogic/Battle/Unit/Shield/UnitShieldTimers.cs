using AkaTimer;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public class UnitShieldTimers
    {
        private readonly Dictionary<uint, Timer> _timers = new Dictionary<uint, Timer>();

        public void Dispose()
        {
            foreach (var timer in _timers)
            {
                AkaLogger.Logger.Instance().Debug("ShieldTimerDispose");
                timer.Value.Dispose();
            }
        }

        public void Pause()
        {
            foreach (var timer in _timers)
            {
                timer.Value.Pause();
            }
        }

        public void Restart(int bulletTime)
        {
            foreach (var timer in _timers)
            {
                timer.Value.Restart(bulletTime);
            }
        }

        public void ReInterval(uint cardId, DateTime endTime)
        {
            if (_timers.ContainsKey(cardId))
            {
                var newInterval = (endTime - DateTime.UtcNow).TotalMilliseconds;
                _timers[cardId].ReInterval(newInterval);
            }
        }

        public void Remove(uint cardId)
        {
            if (_timers.ContainsKey(cardId))
            {
                _timers[cardId].Dispose();
                _timers.Remove(cardId);
            }
        }
    }
}