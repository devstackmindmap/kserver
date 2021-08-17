using AkaData;
using KnightUWP.Dao;
using KnightUWP.Servicecs;
using KnightUWP.UX.Frame;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AkaUtility;
using Windows.UI;
using System.Collections.Specialized;
using Windows.System;
using Windows.Storage;
using AkaEnum;
using AkaConfig;
using StackExchange.Redis;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace KnightUWP.UX.Pages
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class HomePage
    {

        System.Threading.Timer TheTimer = null;
        System.Threading.Timer SyncTimeTimer = null;


        object _battleStartLock = new object();

        public HomePage()
        {
            this.InitializeComponent();

            RootFrame.MainFrame.ShowLoading();

            this.Loaded += async (sender, e) =>
            {

                TheTimer = new System.Threading.Timer(HandleAutoBattleTimer, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                SyncTimeTimer = new System.Threading.Timer(SyncTimer, null, 200, 200);

                await Task.Delay(1000);
                await ProfileManager.LoadDesignDatas();
                ViewModelProvider.UserInfos.CollectionChanged += UserInfos_CollectionChanged;
                await VOProvider.Instance.ReloadUserAccounts();

                ViewModelProvider.UseRedis = Config.BattleServerConfig.GameRedisSetting.Password?.Length > 0;

                var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;

                if (localSetting.Values.TryGetValue("ScheduleTryMatchingInterval", out var outVal))
                    ViewModelProvider.ScheduleTryMatchingInterval = (int)outVal;
                if (localSetting.Values.TryGetValue("ScheduleTryMatchingPerUserInterval", out  outVal))
                    ViewModelProvider.ScheduleTryMatchingPerUserInterval = (int)outVal;
                if (localSetting.Values.TryGetValue("ScheduleTryMatchUserCount", out outVal))
                    ViewModelProvider.ScheduleTryMatchUserCount = (int)outVal;
                if (localSetting.Values.TryGetValue("EnableWrongUser", out outVal))
                    ViewModelProvider.EnableWrongUser = (bool)outVal;

                if (localSetting.Values.TryGetValue("ScheduleRangeMinUserId", out outVal))
                    ViewModelProvider.ScheduleMinUserId = (int)(double)outVal;
                if (localSetting.Values.TryGetValue("ScheduleRangeMaxUserId", out outVal))
                    ViewModelProvider.ScheduleMaxUserId = (int)(double)outVal;


                ViewModelProvider.IsStopScheduling = true;

                UserStateInfo.Navigate(typeof(UserStatePage),null);
                await Task.Delay(1000);
                RootFrame.MainFrame.HideLoading();
                
            };

            this.Unloaded += delegate {
                UserInfo.KeepRun = false;
                TheTimer.Dispose();
                SyncTimeTimer.Dispose();

            };
        }

        private void UserInfos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

            if (ViewModelProvider.UserInfos.Any())
            {
                DispatcherHelper.ExecuteOnUIThreadAsync(() =>
               {
                   Home_UserGridView.ScrollIntoView(ViewModelProvider.UserInfos.Last());

               });
            }
        }

        private async Task Login()
        {
            RootFrame.MainFrame.ShowLoading();
            var selectedUsers = Home_UserGridView.SelectedItems.Cast<UserInfo>().ToArray();

            await Task.Factory.StartNew(() =>
            {
                selectedUsers.AsParallel().ForAll(async userInfo =>
                {
                    await VOProvider.Instance.Login(userInfo);

                });
            });

            RootFrame.MainFrame.HideLoading();
        }


        private async Task Join()
        {
            RootFrame.MainFrame.ShowLoading();

            if (false == VOProvider.Instance.UserAccounts.Any(userAccount => userAccount.socialAccount.ToLower() == AdditionalSocialAccount.ToLower()))
            {
                await VOProvider.Instance.Join(AdditionalSocialAccount);
            }
            else
            {
                //TODO 오류 메시지
            }
            RootFrame.MainFrame.HideLoading();
        }

        private async Task MultipleJoin()
        {
            var sampleNames = Data.GetAiNames();
            var additionalNicks = new List<string>();

            RootFrame.MainFrame.ShowLoading();
            for (int i = 0; i < GenerateNickCount; i++)
            {
                var originName = $"{AkaRandom.Random.ChooseElementRandomlyInCount(sampleNames).Name}";
                var name = originName;

                while (true == VOProvider.Instance.UserAccounts.Any(userAccount => userAccount.nickName.ToLower() == name.ToLower())
                    || additionalNicks.Any(nickName => nickName == name))
                {
                    name = $"{originName}{ new string(Servicecs.Utility.UniqueId().Reverse().ToArray())}";
                    if (name.Length >= 20)
                    {
                        name = name.Substring(0, 19);
                    }                    
                }
                additionalNicks.Add(name);
            }

            await Task.WhenAll(additionalNicks.Select(nickName => VOProvider.Instance.Join(nickName)));

            RootFrame.MainFrame.HideLoading();
        }


        private async Task TryMatchingAndBattleStart()
        {
            //TODO 개별 로딩 & 카운터
            Button_battleStart.Content = "요청을 처리중..";
            Button_battleStart.IsEnabled = false;

            var selectedUsers = Home_UserGridView.SelectedItems.Cast<UserInfo>().Where(userInfo => userInfo.State == UserState.None).ToArray();

            var useRedis = ViewModelProvider.UseRedis;
            if (useRedis == false)
            {
                await Task.WhenAll(selectedUsers.Select(userinfo => VOProvider.Instance.Login(userinfo)));
                selectedUsers = selectedUsers.Where(userInfo => userInfo.LoginInfo.BattlePlayingInfo == null).ToArray();
            }


            
            await Task.WhenAll(selectedUsers.Select(userInfo => VOProvider.Instance.MatchingAndBattleStart(userInfo)));

            Button_battleStart.Content = "선택한 유저 전투";
            Button_battleStart.IsEnabled = true;
        }


        
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Login();
        }


        private async void Button_Battle_Click(object sender, RoutedEventArgs e)
        {
            await TryMatchingAndBattleStart();
        }


        private void SelectUsers(IEnumerable<UserInfo> targetUsers)
        {
            if (targetUsers.Count() > UserAddCounter)
            {
                targetUsers = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(targetUsers, UserAddCounter);
            }

            foreach (var user in targetUsers)
                Home_UserGridView.SelectedItems.Add(user);

        }

        private void ReleaseUsers(IEnumerable<UserInfo> targetUsers)
        {
            if (targetUsers.Count() > UserAddCounter)
            {
                targetUsers = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(targetUsers, UserAddCounter);
            }

            foreach (var user in targetUsers)
                Home_UserGridView.SelectedItems.Remove( user);
        }

        private  void Button_AddTestAccount_Click(object sender, RoutedEventArgs e)
        {
            var targetUsers = 
            ViewModelProvider.UserInfos.Where(userInfo => userInfo.State == UserState.None
                                                        && userInfo.Selected == false
                                                        && userInfo.accounts.socialAccount.IndexOf("@akastudiogroup.com") > 0);
            SelectUsers(targetUsers);
        }
        private  void Button_AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var targetUsers =
            ViewModelProvider.UserInfos.Where(userInfo => userInfo.State == UserState.None
                                                        && userInfo.Selected == false);
            SelectUsers(targetUsers);
        }
        private  void Button_ReleaseTestAccount_Click(object sender, RoutedEventArgs e)
        {
            var targetUsers =
            ViewModelProvider.UserInfos.Where(userInfo => userInfo.State == UserState.None
                                                        && userInfo.Selected == true
                                                        && userInfo.accounts.socialAccount.IndexOf("@akastudiogroup.com") > 0);
            ReleaseUsers(targetUsers);
        }
        private  void Button_ReleaseAccount_Click(object sender, RoutedEventArgs e)
        {
            var targetUsers =
            ViewModelProvider.UserInfos.Where(userInfo => userInfo.State == UserState.None
                                                        && userInfo.Selected == true);
            ReleaseUsers(targetUsers);
        }
        private  void Button_ReleaseNoTestAccount_Click(object sender, RoutedEventArgs e)
        {
            var targetUsers =
            ViewModelProvider.UserInfos.Where(userInfo => userInfo.State == UserState.None
                                                        && userInfo.Selected == true
                                                        && userInfo.accounts.socialAccount.IndexOf("@akastudiogroup.com") < 0);
            ReleaseUsers(targetUsers);
        }










        private void UserGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var userInfo = e.ClickedItem as UserInfo;
            CurrentSelectedUser =  userInfo;
        }

        private void Home_UserGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //check battle playing
            foreach(var userinfo in e.AddedItems.Cast<UserInfo>())
            {
                userinfo.Selected = true;
            }
            foreach (var userinfo in e.RemovedItems.Cast<UserInfo>())
            {
                userinfo.Selected = false;
            }

            this.Bindings.Update();
        }

        private async void Click_Join(object sender, RoutedEventArgs e)
        {
            await Join();
        }

        private async void Click_MultipleJoin(object sender, RoutedEventArgs e)
        {
            await MultipleJoin();
        }

        private void Home_UserGridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var itemContainer = args.ItemContainer as SelectorItem;

            if (args.ItemIndex % 2 == 0)
            {
                itemContainer.Background = new SolidColorBrush(Colors.AliceBlue);
            }
            else
            {
                itemContainer.Background = null;
            }

        //    sender.ScrollIntoView(ViewModelProvider.UserInfos.Last());

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSetting.Values["ScheduleTryMatchingInterval"] = ViewModelProvider.ScheduleTryMatchingInterval;
            localSetting.Values["ScheduleTryMatchingPerUserInterval"] = ViewModelProvider.ScheduleTryMatchingPerUserInterval;
            localSetting.Values["ScheduleTryMatchUserCount"] = ViewModelProvider.ScheduleTryMatchUserCount;
            localSetting.Values["EnableWrongUser"] = ViewModelProvider.EnableWrongUser;

            localSetting.Values["ScheduleRangeMinUserId"] = (double)ViewModelProvider.ScheduleMinUserId;
            localSetting.Values["ScheduleRangeMaxUserId"] = (double)ViewModelProvider.ScheduleMaxUserId;
           

            ViewModelProvider.IsScheduling = true;
            ViewModelProvider.IsStopScheduling = false;

            TheTimer.Change(ViewModelProvider.ScheduleTryMatchingInterval * 1000, ViewModelProvider.ScheduleTryMatchingInterval* 1000);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ViewModelProvider.IsScheduling = false;
            ViewModelProvider.IsStopScheduling = true;

            TheTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

        }


        private void SyncTimer(object state)
        {
            var now = DateTime.UtcNow;
            ViewModelProvider.UserInfos.AsParallel().ForAll(userInfo =>
           {
               if (userInfo.BattleConnecter?.IsConnected ?? false)
               {
                   if (userInfo.State == UserState.BattleEnterRoom)
                   {
                       if((now - userInfo.CurrentBattleInfo.BattleEnterTime).TotalSeconds > 20)
                       {
                           //battle start failed
                           userInfo.State = UserState.None;
                           ViewModelProvider.IncreaseTotalEnterRoomFailedCount();
                           userInfo.WriteLog("Time_EnterRoomFail");
                           userInfo.BattleConnecter.Close();
                       }
                   }
                   else
                   {
                       if ((now - userInfo.LastSyncTimeSended).TotalMilliseconds > 1000 && userInfo.ReceivedSyncTime == true)
                       {
                           ViewModelProvider.SendSyncTime(userInfo);
                       }

                       userInfo.CurrentBattleInfo?.IncreaseElixir();
                       if (userInfo.CurrentBattleInfo?.CanNextDoSkill() ?? false)
                       {
                           ViewModelProvider.SendCardUse(userInfo);
                       }

                   }
               }

           });
        }


        private async void HandleAutoBattleTimer(object state)
        {
            if (ViewModelProvider.IsScheduling == false)
                return;

            var localSetting = Windows.Storage.ApplicationData.Current.LocalSettings;

            var ScheduleTryMatchingInterval = (int)localSetting.Values["ScheduleTryMatchingInterval"];
            var ScheduleTryMatchingPerUserInterval = (int)localSetting.Values["ScheduleTryMatchingPerUserInterval"];
            var ScheduleTryMatchUserCount = (int)localSetting.Values["ScheduleTryMatchUserCount"];
            var EnableWrongUser = (bool)localSetting.Values["EnableWrongUser"];
            var ScheduleRangeMinUserId = (int)(double)localSetting.Values["ScheduleRangeMinUserId"];
            var ScheduleRangeMaxUserId = (int)(double)localSetting.Values["ScheduleRangeMaxUserId"];

            var now = DateTime.UtcNow;

            var shuffleUserInfo = ViewModelProvider.UserInfos.Where(userinfo =>
            {
                return false == (userinfo.accounts.socialAccount.IndexOf("@akastudiogroup.com") < 0
                || userinfo.accounts.userId < ScheduleRangeMinUserId
                || userinfo.accounts.userId > ScheduleRangeMaxUserId
                || (now - userinfo.CurrentBattleInfo.BattleEndTime).TotalSeconds < ViewModelProvider.ScheduleTryMatchingInterval);
            }).OrderBy(notuse => Guid.NewGuid());
            int count = 0;

            var useRedis = ViewModelProvider.UseRedis;
            RedisValue[] result = null;

            if (useRedis)
            {
                var redis = AkaRedis.AkaRedis.GetDatabase();
                AkaRedis.AkaRedis.ConnectCheck(Config.Server);

                var values = shuffleUserInfo.Select(userInfo => (StackExchange.Redis.RedisValue)userInfo.accounts.userId.ToString()).ToArray();
                result = redis.HashGet("HBattlePlayingInfo", values);
            }

            int i = -1;
            foreach (var userinfo in shuffleUserInfo)
            {
                i++;
                if (userinfo.State == UserState.None || (EnableWrongUser && userinfo.State == UserState.BattleWrongClosed))
                {
                    if (useRedis == false)
                    {
                        await VOProvider.Instance.Login(userinfo);
                        if (userinfo.LoginInfo.BattlePlayingInfo != null)
                            continue;
                    }
                    else if ((now - userinfo.CurrentBattleInfo.BattleEndTime).TotalMinutes < 10  && result[i].HasValue)
                    {
                        //늦은 방입장 의심
                        continue;
                    }

                    _ = VOProvider.Instance.MatchingAndBattleStart(userinfo);
                    await Task.Delay(ScheduleTryMatchingPerUserInterval);
                }

                if (++count >= ScheduleTryMatchUserCount)
                    break;
            }
        }

        private async void Button_OpenLogFolder(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }
    }
}
