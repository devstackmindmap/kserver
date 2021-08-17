using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;
using System.Collections.Concurrent;
using SuperSocket.ClientEngine;
using System.Threading;
using AkaEnum.Battle;
using System.Collections.ObjectModel;
using KnightUWP.Servicecs.Protocol;
using Windows.UI.Xaml;
using AkaData;
using System.ComponentModel;

namespace KnightUWP.Dao
{
    public partial class BattleInfo
    {
        SpinLock _lock = new SpinLock();

        double _chargingTime;

        public void ElixirInit()
        {
            _contentsConstant = Data.GetContentsConstant(BattleType.LeagueBattle);
            OriginElixir = _contentsConstant.DefaultElixir;
            var now = DateTime.Now;
            NextUpdateElixirTime = now.AddSeconds(_contentsConstant.ChargingElixirTime);
            LastElixirUpdateTime = now;
            _chargingTime = _contentsConstant.ChargingElixirTime;
        }

        public void ElixirRelease()
        {
            bool gotLock = false;
            _lock.Enter(ref gotLock);
            NextUpdateElixirTime = DateTime.Now.AddYears(10);
            LastElixirUpdateTime = DateTime.Now.AddYears(10);
            OriginElixir = 0;
            if (gotLock) _lock.Exit();
        }

        public void SetBoostTime()
        {
            var now = DateTime.Now;
            bool gotLock = false;
            _lock.Enter(ref gotLock);

            var nextEnahnceRating = 1 - (NextUpdateElixirTime - now).TotalMilliseconds / (_chargingTime * 1000);
            if (_contentsConstant.BoosterElixirMultiple != 0)
                _chargingTime *= _contentsConstant.BoosterElixirMultiple;

            NextUpdateElixirTime = now.AddMilliseconds(_chargingTime * nextEnahnceRating);

            if (gotLock) _lock.Exit();

        }

        public void AddBulletTime(double milSec)
        {

            var now = DateTime.Now;
            bool gotLock = false;
            _lock.Enter(ref gotLock);

            NextUpdateElixirTime = now.AddMilliseconds(milSec);
            if (gotLock) _lock.Exit();

        }

        public void IncreaseElixir()
        {
            var now = DateTime.Now;
            bool gotLock = false;
            _lock.Enter(ref gotLock);

            if (now > NextUpdateElixirTime)
            {
                var overTime = (now - NextUpdateElixirTime).TotalMilliseconds;

                OriginElixir++;
                NextUpdateElixirTime = now.AddSeconds(_chargingTime).AddMilliseconds(-overTime);
                LastElixirUpdateTime = now;
            }

            if (gotLock) _lock.Exit();
        }

        public void AddElixir(double addValue)
        {
            bool gotLock = false;
            _lock.Enter(ref gotLock);

            OriginElixir += addValue;
            if (gotLock) _lock.Exit();
        }


        public void ReplaceElixir(double newValue)
        {
            bool gotLock = false;
            _lock.Enter(ref gotLock);

            OriginElixir = newValue;
            if (gotLock) _lock.Exit();
        }

        public DateTime LastElixirUpdateTime { get; set; }
        public DateTime NextUpdateElixirTime { get; set; }
        double _elixir;

        public int Elixir
        {
            get { return (int)_elixir; }
            set { NotifyPropertyChanged_Status(); }
        }

        public double OriginElixir
        {
            get { return _elixir; }
            set {
                if (value <= 0)
                    _elixir = 0;
                else if (value >= _contentsConstant.MaxElixir)
                    _elixir = _contentsConstant.MaxElixir;
                else
                    _elixir = value;
                Elixir = 0;
            }
        }

    }
}
