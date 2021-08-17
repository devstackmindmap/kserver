using AkaEnum.Battle;
using AkaTimer;
using System;

namespace BattleLogic
{
    public class UnitPoison : IDisposable
    {
        private Unit _unit;
        private int _decreaseValuePerTic;
        private Timer _timer;
        private int _stackedPoison;
        private int _preConsumedPoison;
        private bool _isPoison = false;
        private int _ticMillisecond;

        public PlayerType PlayerType => _unit.PlayerType;
        public int UnitPositionIndex => _unit.UnitData.UnitIdentifier.UnitPositionIndex;
        public int Stack => _isPoison ? _decreaseValuePerTic : 0;

        public bool IsPoison => _isPoison;

        public UnitPoison(Unit unit, int ticSecond)
        {
            _unit = unit;
            _ticMillisecond = 1000 * ticSecond;
            _timer = new Timer(_ticMillisecond, true, OnPoison);
        }

        public void AddPoison(int poison)
        {
            if (!_isPoison)
                PoisonStart();

            _decreaseValuePerTic += poison;
        }

        public void MultiplePoison(float rate)
        {
            if (_isPoison == false)
                return;

            _decreaseValuePerTic = (int)(_decreaseValuePerTic * rate);
        }

        public void OnPoison()
        {
            _stackedPoison += _decreaseValuePerTic;
            EnqueuePoison();
        }

        private void EnqueuePoison()
        {
            _unit.BattleHelper.BattleProgress.EnqueuePoison(this);
        }

        public void PoisonStart()
        {
            _decreaseValuePerTic = 0;
            _stackedPoison = 0;
            _preConsumedPoison = 0;
            _isPoison = true;
        }

        public void PoisonEnd()
        {
            _isPoison = false;
            _timer.Stop();
        }

        public void DecreaseHpByPoison()
        {
            if (_stackedPoison == 0)
                return;

            AkaLogger.Logger.Instance().Debug($"[Poison]Before  Player:{_unit.PlayerType.ToString()} Id:{_unit.UnitData.UnitIdentifier.UnitId} " +
                $"StackedPoison:{_stackedPoison} PreConsumedPoison:{_preConsumedPoison} HP:{_unit.UnitData.UnitStatus.Hp}");

            if (_preConsumedPoison > 0)
                _preConsumedPoison -= _stackedPoison;
            else
                _unit.DecreaseHp(_stackedPoison, AkaEnum.DamageReasonType.Poison, 0, true);

            S2CManager.SendPoison(_unit, _stackedPoison);
            _stackedPoison = 0;

            AkaLogger.Logger.Instance().Debug($"[Poison]After   Player:{_unit.PlayerType.ToString()} Id:{_unit.UnitData.UnitIdentifier.UnitId} " +
                $"StackedPoison:{_stackedPoison} PreConsumedPoison:{_preConsumedPoison} HP:{_unit.UnitData.UnitStatus.Hp}");
        }

        public int GetPreConsumedPoison(int delayMilliseconds)
        {
            if (delayMilliseconds < _ticMillisecond)
                return 0;

            _preConsumedPoison = delayMilliseconds / _ticMillisecond * _decreaseValuePerTic;
            return _preConsumedPoison;
        }

        public int GetCurrentPoisonDamage(int delayMilliseconds)
        {
            if (delayMilliseconds < _ticMillisecond)
                return 0;

            return delayMilliseconds / _ticMillisecond * _decreaseValuePerTic;
        }

        public void Pause()
        {
            if (!_isPoison)
                return;

            _timer.Pause();
        }

        public void Restart(int bulletTime)
        {
            if (!_isPoison)
                return;

            if (_timer.IsTimerStatusStop)
                _timer.Start();
            else
                _timer.Restart(bulletTime);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}
