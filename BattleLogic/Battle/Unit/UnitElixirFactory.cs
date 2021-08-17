using AkaTimer;
using System;

namespace BattleLogic
{
    public class UnitElixirFactory : IDisposable
    {
        private Unit _unit;
        private Timer _timer;
        private int _addElixir;
        private float _interval;
        private bool _isElixir;
        private DateTime _recentAddTime;

        public UnitElixirFactory(Unit unit)
        {
            _unit = unit;
        }

        public void AddElixir(float interval, DateTime startTime)
        {
            if (_timer == null)
            {
                _timer = new Timer(interval, true, OnElixir);
                _timer.Start();
            }
            else
            {
                _timer.ReInterval(interval);
            }

            _interval = interval;
            _recentAddTime = startTime;
            _addElixir = 1;
            _isElixir = true;
        }

        private void OnElixir()
        {
            _unit.BattleHelper.EnqueueAddElixir(new BattleActionAddElixir(_unit, _addElixir, _recentAddTime));
            _recentAddTime = _recentAddTime.AddMilliseconds(_interval);
        }

        public void ElixirFactoryEnd()
        {
            _timer.Stop();
            _isElixir = false;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public void Pause()
        {
            if (_isElixir == false)
                return;

            _timer.Pause();
        }

        public void Restart(int bulletTime)
        {
            if (_isElixir == false)
                return;

            if (_timer.IsTimerStatusStop)
                _timer.Start();
            else
                _timer.Restart(bulletTime);

            _recentAddTime = _recentAddTime.AddMilliseconds(bulletTime);
        }
    }
}