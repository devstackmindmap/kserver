using AkaEnum;
using AkaTimer;
using System;

namespace BattleLogic
{
    public class Elixir : IDisposable
    {
        private readonly double _chargingElixirTime;
        private object _lockCharge = new object();
        private ElixirValue _elixirValue;
        private Timer _chargingTimer;
        private EnqueueSkillReservation _enqueueSkillReservation;
        private DateTime _recentOnChargingTime;

        private readonly float _boosterElixirMultiple;

        public double CurrentElixir => _elixirValue.CurrentElixir;
        public long RecentOnChargingTime => _recentOnChargingTime.Ticks;

        public Elixir(BattleEnviroment enviroment)
        {
            _chargingElixirTime = enviroment.ChargingElixirTime;
            _boosterElixirMultiple = enviroment.BoosterElixirMultiple;
            _elixirValue = new ElixirValue(enviroment.DefaultElixir, enviroment.MaxElixir, _chargingElixirTime);
            _chargingTimer = new Timer(_chargingElixirTime * 10, true, OnChargingTime);
            _chargingTimer.Name = "ElixirTimer";
            _enqueueSkillReservation = new EnqueueSkillReservation(_elixirValue);
        }

        public void OnChargingTime()
        {
            lock (_lockCharge)
            {
                var deltaTime = (DateTime.UtcNow - _recentOnChargingTime).TotalSeconds;
                _elixirValue.ChargeElixir(deltaTime);
                CheckReservation();
            }

            _recentOnChargingTime = DateTime.UtcNow;
        }

        public ElixirCountStateData GetElixirCountState(int needElixir)
        {
            if (needElixir <= _elixirValue.CurrentElixir)
                return new ElixirCountStateData(ElixirCountState.Enough, _elixirValue.CurrentElixir, needElixir);
            else if (needElixir - 1 > _elixirValue.CurrentElixir || _enqueueSkillReservation.HasReservation())
                return new ElixirCountStateData(ElixirCountState.NotEnough, _elixirValue.CurrentElixir, needElixir);
            else
                return new ElixirCountStateData(ElixirCountState.CardUseReservation, _elixirValue.CurrentElixir, needElixir);
        }

        public void UseElixir(int useCount)
        {
            lock (_lockCharge)
            {
                _elixirValue.UseElixir(useCount);
            }
        }

        public void Start(DateTime battleStartTime)
        {
            _recentOnChargingTime = battleStartTime;
            _chargingTimer.Start();
        }

        public void Pause()
        {
            _chargingTimer.Pause();
        }

        public void Stop()
        {
            _chargingTimer.Stop();
        }

        public void Restart(int bulletTime)
        {
            _recentOnChargingTime = _recentOnChargingTime.AddMilliseconds(bulletTime);
            _chargingTimer.Restart(bulletTime);
        }

        public void EnqueueSkillReservation(Action<CardUseActionData> enqueueSkillAction, CardUseActionData cardUseActionData)
        {
            _enqueueSkillReservation.Reservation(enqueueSkillAction, cardUseActionData);
        }

        public void Dispose()
        {
            _chargingTimer.Dispose();
        }

        public void StartBooster()
        {
            _elixirValue.StartBooster(_boosterElixirMultiple);
        }

        public void AddElixir(int elixir)
        {
            lock (_lockCharge)
            {
                _elixirValue.AddElixir(elixir);
                CheckReservation();
            }
        }

        private void CheckReservation()
        {
            if (_enqueueSkillReservation.IsDoReservation())
                _enqueueSkillReservation.EnqueueSkill();
        }
    }
}
