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
using Microsoft.Toolkit.Uwp.Helpers;
using AkaUtility;
using KnightUWP.Servicecs;

namespace KnightUWP.Dao
{
    public partial class BattleInfo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        DataContentsConstant _contentsConstant;

        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
           {
               PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
           });
        }

        private void NotifyPropertyChanged_Status([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (VOProvider.Instance.EnableClientState == false)
                return;

            NotifyPropertyChanged(propertyName);
        }



        public BattleInfo()
        {
            FilteredHistory = new ObservableCollection<AbstractBattleProcess>();
            History = new ObservableCollection<AbstractBattleProcess>();
            VisibileHistory = History;

            MyUnits = new ObservableCollection<UnitInfo>();
            EnemyUnits = new ObservableCollection<UnitInfo>();
            BattleEndTime = DateTime.UtcNow.AddDays(-1);
            BattleEnterTime = DateTime.UtcNow;
            ElixirRelease();
        }



        public ObservableCollection<UnitInfo> MyUnits { get; set; }
        public ObservableCollection<UnitInfo> EnemyUnits { get; set; }

        public ObservableCollection<AbstractBattleProcess> History { get; set; }
        public ObservableCollection<AbstractBattleProcess> FilteredHistory { get; set; }



        ObservableCollection<AbstractBattleProcess> _VisibileHistory;
        public ObservableCollection<AbstractBattleProcess> VisibileHistory
        {
            get { return _VisibileHistory; }
            set { _VisibileHistory = value; NotifyPropertyChanged_Status(); }
        }



        public bool IsPvP { get; set; }


        public PlayerType MyPlayer { get; set; }
        public PlayerType EnemyPlayer { get; set; }

        public ProtoBeforeBattleStart BeforeBattleStartInfo { get; set; }

        public DateTime BattleStartTime { get; set; }


        public DateTime BattleEnterTime { get; set; }

        public DateTime BattleEndTime { get; set; }

        string _battleTime;
        public string BattleTime
        {
            get { return _battleTime == null ? "" : _battleTime; }
            set { _battleTime = value; NotifyPropertyChanged_Status(); }
        }

        uint _card1, _card2, _card3, _card4;

        public uint Card1
        {
            get { return _card1; }
            set { _card1 = value; NotifyPropertyChanged_Status(); }
        }
        public uint Card2
        {
            get { return _card2; }
            set { _card2 = value; NotifyPropertyChanged_Status(); }
        }
        public uint Card3
        {
            get { return _card3; }
            set { _card3 = value; NotifyPropertyChanged_Status(); }
        }
        public uint Card4
        {
            get { return _card4; }
            set { _card4 = value; NotifyPropertyChanged_Status(); }
        }

        int _needElixirForNextSkill;
        uint _nextCard;
        uint _nextCardStatId;
        public uint NextCard
        {
            get { return _nextCard; }
            set
            {
                _nextCard = value;

                switch (_nextCard)
                {
                    case 0: _nextCardStatId = _card1; break;
                    case 1: _nextCardStatId = _card2; break;
                    case 2: _nextCardStatId = _card3; break;
                    case 3: _nextCardStatId = _card4; break;
                }

                _needElixirForNextSkill = Data.GetCardStat(_nextCardStatId)?.Elixir ?? 0;
                NotifyPropertyChanged_Status();
            }
        }

        public uint NextCardStatId  {
            get {
                switch (_nextCard)
                {
                    case 0: return _card1; 
                    case 1: return _card2; 
                    case 2: return _card3; 
                    case 3: return _card4; 
                }
                return _card1;
            }
        }
        public int UseCount { get; set; }

       // public bool UsedCard { get; set; }


        public bool CanNextDoSkill()
        {
            if (Elixir > _needElixirForNextSkill + 1 && UseCount-- < 0)
            {
                return true;
            }
            return false;
        }

        public void SetNextCard()
        {
            var cards = new uint[] { _card1, _card2, _card3, _card4 }.Select( (card,index) => card == 0 ? 99 : index).Where(index => index != 99);

            if (cards.Any())
            {
                lock(this)
                {
                    NextCard = (uint)AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(cards, 1).First();

                    var min = 1;
                    var max = 5;// (10 - Elixir) * _chargingTime + 2;
                    UseCount = (int)AkaRandom.Random.NextUint((uint)min, (uint)max) * 5;
                }
            }
        }
    }
}
