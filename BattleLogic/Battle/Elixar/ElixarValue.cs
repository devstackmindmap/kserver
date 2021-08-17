using System;

namespace BattleLogic
{
    public class ElixirValue
    {
        private float _maxElixir;
        private float _defaultElixir;
        private double _currentElixir;
        private double _chargingTime;
        private float _boosterElixirMultiple = 1;

        public double CurrentElixir { get { return _currentElixir; } }

        public ElixirValue(float defaultElixir, float maxElixir, double chargingTime)
        {
            _maxElixir = maxElixir;
            _defaultElixir = defaultElixir;
            _currentElixir = _defaultElixir;
            _chargingTime = chargingTime;
        }

        public void ChargeElixir(double deltaTime)
        {
            AddElixir(deltaTime / (_chargingTime * _boosterElixirMultiple));
        }

        public void UseElixir(int useCount)
        {
            _currentElixir -= useCount;
        }

        public void AddElixir(double elixir)
        {
            if (_currentElixir < _maxElixir)
                _currentElixir = Math.Min(_currentElixir + elixir, _maxElixir);
        }

        public void StartBooster(float boosterElixirMultiple)
        {
            _boosterElixirMultiple = boosterElixirMultiple;
        }
    }
}
