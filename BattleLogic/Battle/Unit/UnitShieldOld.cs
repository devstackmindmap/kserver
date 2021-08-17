using AkaEnum;
using AkaTimer;
using System;
using PlayerType = AkaEnum.Battle.PlayerType;

namespace BattleLogic
{
    public class UnitShieldOld : IDisposable
    {
        private Unit _unit;
        private int _shield;
        private int _decreaseValuePerSecond;
        private Timer _timer;
        private int _stackedDecreaseShield;
        private int _preConsumedShield;
        private bool _isShield = false;
        private DateTime _tickStartTime;

        public PlayerType PlayerType => _unit.PlayerType;
        public int UnitPositionIndex => _unit.UnitData.UnitIdentifier.UnitPositionIndex;

        public int Shield => _shield;
        public bool IsShield => _isShield;


        public UnitShieldOld(Unit unit, int decreaseValuePerSecond)
        {
            _unit = unit;
            _timer = new Timer(1000, true, AddDecreaseShield);
            _decreaseValuePerSecond = (int)(decreaseValuePerSecond - (decreaseValuePerSecond * unit.UnitData.UnitStatus.ShieldReduceRate));
            AddShield((int)unit.UnitData.UnitStatus.ShieldStart, true);
        }

        public void AddDecreaseShield()
        {
            if (_unit.IsContainConditionBuffAndNotValidRemove(SkillEffectType.BUFF_STATE_SHIELD_FREEZE))
            {
                _tickStartTime = DateTime.UtcNow;
                return;
            }

            _stackedDecreaseShield += _decreaseValuePerSecond;
            EnqueueShield();
            _tickStartTime = DateTime.UtcNow;
        }

        private void EnqueueShield()
        {
            _unit.BattleHelper.BattleProgress.EnqueueShield(this);
        }

        public void AddShield(int shield, bool isBattleStart = false)
        {
            if (isBattleStart == false)
                shield = (int)(shield * (1 + _unit.UnitData.UnitStatus.ShieldGenerationRate));

            if (_shield > 0)
                _shield += shield;
            else
                ShieldStart(shield);

            //AkaLogger.Log.Battle.Shiled.AddShieldLog(_unit.MyPlayer.Battle.RoomId, _unit.PlayerType, _unit.UnitData.UnitIdentifier.UnitId, _shield - shield, shield);

        }

        public void ShieldStart(int shield)
        {
            _shield = 0;
            _shield += shield;
            _stackedDecreaseShield = 0;
            _preConsumedShield = 0;
            _isShield = true;
            _tickStartTime = DateTime.UtcNow;
        }

        public void ShieldEnd()
        {
            _shield = 0;
            _isShield = false;
            _timer.Stop();
            //AkaLogger.Log.Battle.Shiled.ShieldEndLog(_unit.MyPlayer.Battle.RoomId, _unit.PlayerType, _unit.UnitData.UnitIdentifier.UnitId);
        }

        public void DecreaseShield()
        {
            if (_stackedDecreaseShield == 0)
                return;

            var logPrevShield = _shield;
            var logstackedDecreaseShield = _stackedDecreaseShield;
            var logPreConsumedShield = _preConsumedShield;

            _shield -= _stackedDecreaseShield;

            _stackedDecreaseShield = 0;

            if (IsShieldEnd())
                ShieldEnd();

            //AkaLogger.Logger.Instance().Info($"[Shield]After   Player:{_unit.PlayerType.ToString()} Id:{_unit.UnitData.UnitIdentifier.UnitId} " +
            //    $"Shield:{_shield} StackedDecreaseShield:{_stackedDecreaseShield} PreConsumedShield:{_preConsumedShield}");
        }

        public bool IsShieldEnd()
        {
            return _shield <= 0;
        }

        public void UpdateCurrentShield(int delayMilliseconds)
        {
            var calcShield = GetShieldCalculateByBetweenTick();
            if (delayMilliseconds > 0)
            {
                _shield = (int)GetShieldCalculateByDelayMilliseconds(calcShield, delayMilliseconds);
                AfterDelayStart(delayMilliseconds);
            }
            else
            {
                ResetTimer();
            }
        }

        public (float ExtraDamage, float realDamage) GetExtraDamageAndDoDamageToShield(float damage, float shieldDamageRate)
        {
            damage = GetDamageApplyShieldDamageRate(damage, shieldDamageRate);
            if (_shield <= 0)
                return (damage, damage);

            //AkaLogger.Log.Battle.Shiled.ShieldDamageLog(_unit.MyPlayer.Battle.RoomId, _unit.PlayerType, _unit.UnitData.UnitIdentifier.UnitId, _shield, damage);
            _shield -= (int)damage;
            if (_shield < 0)
            {
                var extraDamage = _shield * -1;
                ShieldEnd();
                return (extraDamage, damage);
            }
            return (0, damage);
        }

        private float GetDamageApplyShieldDamageRate(float damage, float shieldDamageRate)
        {
            if (_shield > 0)
                return damage * shieldDamageRate;

            return damage;
        }

        private void AfterDelayStart(int delayMilliseconds)
        {
            _timer.Stop();
            _timer.SetAttribute(delayMilliseconds, false, ResetTimer);
            _timer.Start();
        }

        public void Pause()
        {
            if (!_isShield)
                return;

            _timer.Pause();
        }

        public void Restart(int bulletTime)
        {
            if (!_isShield)
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

        public void AddBulletTime(int millisecond)
        {
            _tickStartTime.AddMilliseconds(millisecond);
        }

        private double GetShieldCalculateByDelayMilliseconds(double shield, double delayMilliseconds)
        {
            return shield - (delayMilliseconds / 1000 * _decreaseValuePerSecond);
        }

        private double GetShieldCalculateByBetweenTick()
        {
            return _shield - ((DateTime.UtcNow - _tickStartTime).TotalMilliseconds / 1000 * _decreaseValuePerSecond);
        }

        private void ResetTimer()
        {
            _timer.Stop();
            _timer.SetAttribute(1000, true, AddDecreaseShield);

            if (!_timer.IsTimerStatusPause)
            {
                _timer.Start();
            }
        }

        public void TimerStartByBattleStart()
        {
            _timer.Start();
        }

        internal void SetReduceRate(float boostReduceShieldRate)
        {
            _unit.UnitData.UnitStatus.ShieldReduceRate = boostReduceShieldRate;
            _decreaseValuePerSecond = (int)(_decreaseValuePerSecond * _unit.UnitData.UnitStatus.ShieldReduceRate);
        }
    }
}
