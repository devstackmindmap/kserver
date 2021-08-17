using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaUtility;
using CommonProtocol;
using KnightUWP.Dao;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Servicecs
{
    public sealed partial class VOProvider
    {
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            NotifyPropertyChangedEx(propertyName);
        }

        public void NotifyPropertyChangedEx(string propertyName)
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            });
        }


        public int SelectedUserCount => UserInfos.Count(userInfo => userInfo.Selected && userInfo.accounts.socialAccount.IndexOf("@akastudiogroup.com") < 0);
        public int SelectedTestUserCount => UserInfos.Count(userInfo => userInfo.Selected && userInfo.accounts.socialAccount.IndexOf("@akastudiogroup.com") > 0);

        public int InRoomUserCount => UserInfos.Count(userInfo => userInfo.State.In(UserState.BattleBeforeStart , UserState.BattleStart) );
        public int InAiRoomUserCount => UserInfos.Count(userInfo => userInfo.State.In(UserState.BattleBeforeStart, UserState.BattleStart) && userInfo.CurrentBattleInfo.IsPvP == false);
        public int TryMatchingUsercount => UserInfos.Count(userInfo => userInfo.State.In(UserState.Matching, UserState.Matched, UserState.BattleEnterRoom));

        public int WrongBattleClosedcount => UserInfos.Count(userInfo => userInfo.State.In(UserState.BattleWrongClosed));

        SpinLock _lockTotalTryMatchingCount = new SpinLock();
        SpinLock _lockTotalEndBattleCount = new SpinLock();
        SpinLock _lockEnterRoomFailedCount = new SpinLock();
        SpinLock _lockBattleConnectionFailedCount = new SpinLock();
        SpinLock _lockTotalMatchingFailedCount = new SpinLock();

        public void IncreaseTotalEndBattleCount()
        {
            bool gotLock = false;
            _lockTotalEndBattleCount.Enter(ref gotLock);
            TotalEndBattleCount++;
            if (gotLock) _lockTotalEndBattleCount.Exit();
        }

        public void IncreaseTotalTryMatchingCount()
        {
            bool gotLock = false;
            _lockTotalTryMatchingCount.Enter(ref gotLock);
            TotalTryMatchingCount++;
            if (gotLock) _lockTotalTryMatchingCount.Exit();
        }


        public void IncreaseTotalTryMatchingFailCount()
        {
            bool gotLock = false;
            _lockTotalMatchingFailedCount.Enter(ref gotLock);
            _TotalMatchingFailedCount++;
            if (gotLock) _lockTotalMatchingFailedCount.Exit();
        }

        public void IncreaseTotalBattleConnecFailCount()
        {
            bool gotLock = false;
            _lockBattleConnectionFailedCount.Enter(ref gotLock);
            TotalBattleConnectionFailedCount++;
            if (gotLock) _lockBattleConnectionFailedCount.Exit();

        }

        public void IncreaseTotalEnterRoomFailedCount()
        {
            bool gotLock = false;
            _lockEnterRoomFailedCount.Enter(ref gotLock);
            TotalEnterRoomFailedCount++;
            if (gotLock) _lockEnterRoomFailedCount.Exit();
        }


        int _TotalTryMatchingCount;
        public int TotalTryMatchingCount
        {
            get { return _TotalTryMatchingCount; }
            set { _TotalTryMatchingCount = value; NotifyPropertyChanged(); }

        }

        int _TotalMatchingFailedCount;
        public int TotalMatchingFailedCount
        {
            get { return _TotalMatchingFailedCount; }
            set { _TotalMatchingFailedCount = value; NotifyPropertyChanged(); }

        }

        int _TotalEnterRoomFailedCount;
        public int TotalEnterRoomFailedCount
        {
            get { return _TotalEnterRoomFailedCount; }
            set { _TotalEnterRoomFailedCount = value; NotifyPropertyChanged(); }

        }

        int _TotalBattleConnectionFailedCount;
        public int TotalBattleConnectionFailedCount
        {
            get { return _TotalBattleConnectionFailedCount; }
            set { _TotalBattleConnectionFailedCount = value; NotifyPropertyChanged(); }

        }

        int _TotalEndBattleCount;
        public int TotalEndBattleCount
        {
            get { return _TotalEndBattleCount; }
            set { _TotalEndBattleCount = value; NotifyPropertyChanged(); }

        }

        int _ScheduleTryMatchUserCount = 1;
        public int ScheduleTryMatchUserCount
        {
            get { return _ScheduleTryMatchUserCount; }
            set { _ScheduleTryMatchUserCount = value; NotifyPropertyChanged(); }

        }

        int _ScheduleTryMatchingInterval = 30;
        public int ScheduleTryMatchingInterval
        {
            get { return _ScheduleTryMatchingInterval; }
            set { _ScheduleTryMatchingInterval = value; NotifyPropertyChanged(); }

        }

        int _ScheduleTryMatchingPerUserInterval = 1;
        public int ScheduleTryMatchingPerUserInterval
        {
            get { return _ScheduleTryMatchingPerUserInterval; }
            set { _ScheduleTryMatchingPerUserInterval = value; NotifyPropertyChanged(); }

        }

        bool _EnableClientState = false;
        public bool EnableClientState
        {
            get { return _EnableClientState; }
            set { _EnableClientState = value; NotifyPropertyChanged(); }

        }

        bool _EnablePubsub = false;
        public bool EnablePubsub
        {
            get { return _EnablePubsub; }
            set { _EnablePubsub = value; NotifyPropertyChanged(); }

        }

        bool _EnableMatchingTest = false;
        public bool EnableMatchingTest
        {
            get { return _EnableMatchingTest; }
            set { _EnableMatchingTest = value; NotifyPropertyChanged(); }

        }

        bool _EnableWrongUser = false;
        public bool EnableWrongUser
        {
            get { return _EnableWrongUser; }
            set { _EnableWrongUser = value; NotifyPropertyChanged(); }

        }

        bool _UseRedis = true;
        public bool UseRedis
        {
            get { return _UseRedis; }
            set { _UseRedis = value; NotifyPropertyChanged(); }

        }

        bool _isScheduling;
        public bool IsScheduling
        {
            get { return _isScheduling; }
            set { _isScheduling = value; NotifyPropertyChanged(); }

        }

        bool _isStopScheduling;
        public bool IsStopScheduling
        {
            get { return _isStopScheduling; }
            set { _isStopScheduling = value; NotifyPropertyChanged(); }

        }


        int _ScheduleMinUserId = 1;
        public int ScheduleMinUserId
        {
            get { return _ScheduleMinUserId; }
            set { _ScheduleMinUserId = value; NotifyPropertyChanged(); }

        }

        int _ScheduleMaxUserId = 1;
        public int ScheduleMaxUserId
        {
            get { return _ScheduleMaxUserId; }
            set { _ScheduleMaxUserId = value; NotifyPropertyChanged(); }

        }

        bool _EnableEnqueueActionLog = true;
        public bool EnableEnqueueActionLog
        {
            get { return _EnableEnqueueActionLog; }
            set { _EnableEnqueueActionLog = value; OnEnableEnqueueActionLogChanged();  NotifyPropertyChanged(); }

        }

        
        private void OnEnableEnqueueActionLogChanged()
        {
            var pThis = this;

            foreach (var userInfo in pThis.UserInfos)
                if (userInfo.CurrentBattleInfo != null)
                    userInfo.CurrentBattleInfo.VisibileHistory = pThis.EnableEnqueueActionLog ? userInfo.CurrentBattleInfo.History : userInfo.CurrentBattleInfo.FilteredHistory;
          
        }
    }
}
