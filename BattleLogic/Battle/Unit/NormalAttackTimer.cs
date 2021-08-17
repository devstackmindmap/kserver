using AkaLogger;
using AkaTimer;
using System;

namespace BattleLogic
{
    public class NormalAttackTimer : Timer
    {
        public double UnitInterval;

        private double _nowAttackSpeed;

        private double _elapsedTimeMilli => (DateTime.UtcNow - _recentEventDateTime).TotalMilliseconds;

        public NormalAttackTimer(double interval, bool autoReset, Action action) : base(interval, autoReset, action)
        {
            UnitInterval = interval;
            _nowAttackSpeed = interval;
        }

        public void RecoveryInterval(UnitActionStatus unitActionStatus)
        {
            ChangeInterval((float)UnitInterval, unitActionStatus);
        }

        public void ChangeInterval(float newAttackInterval, UnitActionStatus unitActionStatus)
        {
            Logger.Instance().Debug(Name + " : [ChangeInterval:{0}]", newAttackInterval);
            if (unitActionStatus == UnitActionStatus.EnqueueAttack)
            {
                SetAttribute(newAttackInterval, false);
                Logger.Instance().Debug("[tempInverval:{0}]", newAttackInterval);
                return;
            }

            Pause();
            var oneTemporaryInterval = GetNewInterval(newAttackInterval);
            IntervalAfterOneTemporary = newAttackInterval;
            Restart(0, oneTemporaryInterval);
            _nowInterval = oneTemporaryInterval;
            _nowAttackSpeed = newAttackInterval;

            Logger.Instance().Debug("[tempInverval:{0}]", oneTemporaryInterval);
        }

        public double GetNewInterval(double newAttackInterval)
        {
            var rate = Math.Min(Math.Max(_nowInterval - _elapsedTimeMilli, 0) / _nowAttackSpeed, 1);
            Logger.Instance().Debug("[GetNewInterval:{0},  ElapsedTime:{1},  NowInterval:{2},  Rate:{3},  NowAttackSpeed:{4}]", newAttackInterval, _elapsedTimeMilli, _nowInterval, rate, _nowAttackSpeed);
            return newAttackInterval * rate;
        }
        public long NextAttackTime()
        {
            return _recentEventDateTime.AddMilliseconds(_nowInterval).Ticks;
        }
    }
}
