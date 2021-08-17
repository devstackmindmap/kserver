using AkaEnum;
using AkaTimer;
using AkaUtility;
using System;
using System.Collections.Generic;

namespace BattleLogic
{
    public class UnitBuffTimers : IDisposable
    {
        private readonly Dictionary<SkillEffectType, Timer> _buffTimers = new Dictionary<SkillEffectType, Timer>(SkillEffectTypeComparer.Comparer);

        public void Dispose()
        {
            foreach (var timer in _buffTimers)
            {
                AkaLogger.Logger.Instance().Debug("BuffTimerDispose");
                timer.Value.Dispose();
            }
        }

        public void AddBuff(SkillEffectType skillEffectType, double interval, Action BuffEnd)
        {
            var buffTimer = new Timer(interval, false, BuffEnd);
            _buffTimers.Add(skillEffectType, buffTimer);
            _buffTimers[skillEffectType].Name = "AS";
            buffTimer.Start();
        }

        public void RemoveBuff(SkillEffectType skillEffectType)
        {
            if (_buffTimers.ContainsKey(skillEffectType))
            {
                _buffTimers[skillEffectType].Dispose();
                _buffTimers.Remove(skillEffectType);
            }
        }

        public void RemoveBuffs()
        {
            foreach (var buffTimer in _buffTimers)
            {
                buffTimer.Value.Dispose();
            }

            _buffTimers.Clear();
        }

        public void RemoveBuffs(List<SkillEffectType> removedTypes)
        {
            for (var i = 0; i < removedTypes.Count; i++)
            {
                if (_buffTimers.ContainsKey(removedTypes[i]) == false)
                    continue;

                _buffTimers[removedTypes[i]].Dispose();
                _buffTimers.Remove(removedTypes[i]);
            }
        }

        public void Pause()
        {
            foreach (var timer in _buffTimers)
            {
                timer.Value.Pause();
            }
        }

        public void Restart(int bulletTime)
        {
            foreach (var timer in _buffTimers)
            {
                timer.Value.Restart(bulletTime);
            }
        }

        public void ReInterval(SkillEffectType skillEffectType, DateTime EndTime)
        {
            if (_buffTimers.ContainsKey(skillEffectType))
            {
                var newInterval = (EndTime - DateTime.UtcNow).TotalMilliseconds;
                _buffTimers[skillEffectType].ReInterval(newInterval);
            }
        }
    }
}
