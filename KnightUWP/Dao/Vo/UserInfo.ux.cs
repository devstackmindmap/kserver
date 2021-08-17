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
using Windows.UI.Xaml;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.Helpers;
using AkaData;
using KnightUWP.Servicecs;
using Windows.UI.Xaml.Media;
using AkaUtility;
using KnightUWP.Servicecs.Net;

namespace KnightUWP.Dao
{
    public partial class UserInfo
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChangedEx(string propertyName)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            });
        }
        protected void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            NotifyPropertyChangedEx(propertyName);
        }

        private void NotifyPropertyChanged_Status([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (VOProvider.Instance.EnableClientState == false)
                return;

            NotifyPropertyChangedEx(propertyName);
        }


        public bool Selected { get; set; }

        BattleInfo _currentBattleInfo;
        public BattleInfo CurrentBattleInfo
        {
            get { return _currentBattleInfo; }
            set { _currentBattleInfo = value; NotifyPropertyChanged(); }

        }

        UserState _State = UserState.None;
        public UserState State
        {
            get { return _State; }
            set {
                var oldVal = _State;
                _State = value;
                OnStatePropertyChanged(value, oldVal);
                NotifyPropertyChanged();

                if (_State == UserState.Matching)
                    Pubsub.Matching(this);
                else if (_State == UserState.BattleStart)
                    Pubsub.Battle(this);
                else if (_State.In(UserState.BattleWrongClosed, UserState.None))
                    Pubsub.Online(this);
            }

        }

        int _MaxLatency ;
        public int MaxLatency
        {
            get { return _MaxLatency; }
            set { _MaxLatency = value; NotifyPropertyChanged_Status(); }

        }

        int _AverageLatency;
        public int AverageLatency
        {
            get { return _AverageLatency; }
            set { _AverageLatency = value; NotifyPropertyChanged_Status(); }

        }

        string _CurrentBattleTime = "";
        public string CurrentBattleTime
        {
            get { return _CurrentBattleTime; }
            set { _CurrentBattleTime = value; NotifyPropertyChanged_Status(); }

        }





        private void OnStatePropertyChanged(UserState newState, UserState oldState )
        {
            var pThis = this;

            VOProvider.Instance.NotifyPropertyChangedEx("InRoomUserCount");
            VOProvider.Instance.NotifyPropertyChangedEx("InAiRoomUserCount");
            VOProvider.Instance.NotifyPropertyChangedEx("TryMatchingUsercount");
            VOProvider.Instance.NotifyPropertyChangedEx("WrongBattleClosedcount");

            if (newState == UserState.BattleBeforeStart && oldState != UserState.BattleBeforeStart)
            {
                pThis.NotifyPropertyChanged_Status("EnemyUnit1");
                pThis.NotifyPropertyChanged_Status("EnemyUnit2");
                pThis.NotifyPropertyChanged_Status("EnemyUnit3");
                pThis.NotifyPropertyChanged_Status("MyUnit1");
                pThis.NotifyPropertyChanged_Status("MyUnit2");
                pThis.NotifyPropertyChanged_Status("MyUnit3");

                pThis.NotifyPropertyChanged_Status("EnemyNickName");
                pThis.NotifyPropertyChanged_Status("EnemyUserId");
                pThis.NotifyPropertyChanged_Status("EnemyRankPoint");
            }           
        }



        public string EnemyNickName
        {
            get
            {
                return CurrentBattleInfo?.BeforeBattleStartInfo?.EnemyPlayer.Nickname ?? "";
            }

        }

        public uint EnemyUserId
        {
            get { return CurrentBattleInfo?.BeforeBattleStartInfo?.EnemyPlayer.UserId ?? 0; }
        }

        public uint EnemyRankPoint
        {
            get { return (uint)(CurrentMatchingInfo?.EnemyUserRankPoint ?? 0 ); }
        }


    }
}
