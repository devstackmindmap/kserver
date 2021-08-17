using AkaDB.MySql;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Servicecs
{
    public sealed partial class VOProvider : System.ComponentModel.INotifyPropertyChanged
    {

        public ObservableCollection<Accounts> UserAccounts { get; private set; }
     //   public ObservableCollection<Users> UserStatus { get; private set; }

        public ObservableCollection<UserInfo> UserInfos { get; private set; }

        private Dictionary<uint,Users> _userStatus = new Dictionary<uint, Users>();


        public static VOProvider Instance => _instance;

        protected static VOProvider _instance = new VOProvider();
        protected VOProvider()
        {
            UserAccounts = new ObservableCollection<Accounts>();
            UserInfos = new ObservableCollection<UserInfo>();
          //  UserStatus = new ObservableCollection<Users>();

            UserAccounts.CollectionChanged += UserAccounts_CollectionChanged;
        }

        void ClearUsers()
        {
            UserAccounts.Clear();
        //    UserStatus.Clear();
            UserInfos.Clear();
        }

        public async Task ReloadUserAccounts()
        {
            ClearUsers();
            List<Accounts> accounts = new List<Accounts>();
            await Task.Factory.StartNew(async () =>
            {
                accounts = await Accounts.GetAllObjects(0);

                var dbContextList = accounts.GroupBy(account => AkaConfig.Config.GetShardNum(account.userId));
                var shardIdList = dbContextList.Select(dbContextWithUsers => dbContextWithUsers.AsEnumerable().First());

                foreach (var shardId in shardIdList)
                {
                    var users = await Users.GetAllObjects(shardId.userId);

                    foreach (var user in users)
                    {
                        _userStatus.Add(user.userId, user);
                    }
                }
            });

            foreach (var account in accounts)
            {
                UserAccounts.Add(account);
            }
        }

        public async Task ReloadUserAccount( uint userId )
        {
            var account = await Accounts.GetObject(0, $"WHERE userId = {userId.ToString()}");
            var user = await Users.GetObject(userId, $"WHERE userId = {userId.ToString()}");

            if (true == _userStatus.ContainsKey(userId))
            {
                _userStatus[userId] = user;
                var foundUser = UserAccounts.FirstOrDefault( userAccount => userAccount.userId == userId);
                var idx = UserAccounts.IndexOf(foundUser);
                UserAccounts[idx] = account;
            }
            else
            {
                _userStatus.Add(userId, user);
                UserAccounts.Add(account);
            }
        }


        private void UserAccounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Accounts item in e.NewItems)
                    {
                        if (_userStatus.TryGetValue(item.userId, out var user))
                        {
                            UserInfos.Add(new UserInfo { accounts = item, users = user });
                        }
                        else
                        {
                            var newUser = new Users();
                            if (item.userId != 0)
                                _userStatus.Add(item.userId, newUser);

                            //TODO userid가 0일 경우 ui처리 (올바르지 않은 데이터)
                            UserInfos.Add(new UserInfo { accounts = item, users = newUser });
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removedAccounts = e.NewItems.Cast<Accounts>();
                    var removeTargets = UserInfos.Where(userinfo => removedAccounts.Any(account => account.userId == userinfo.accounts.userId)).ToArray();
                   
                    foreach (var userInfo in removeTargets)
                    {
                        UserInfos.Remove(userInfo);
                        _userStatus.Remove(userInfo.users.userId);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    var newAccounts = e.NewItems.Cast<Accounts>();

                    foreach (var userAccount in newAccounts)
                    {
                        var user = UserInfos.FirstOrDefault(userInfo => userInfo.accounts.userId == userAccount.userId);
                        user.accounts = userAccount;
                        user.users = _userStatus[userAccount.userId];
                    }

                    break;
            }

            if (UserInfos.Any())
            {
                var userids = UserInfos.Select(userInfo => userInfo.accounts.userId);
                ScheduleMinUserId = (int)userids.Min();
                ScheduleMaxUserId = (int)userids.Max();
            }
        }





    }
}
