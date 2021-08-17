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

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace KnightUWP.UX.Pages
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class HomePage
    {
        const int UserAddCounter = 10;

        public VOProvider ViewModelProvider = VOProvider.Instance;

        private string AdditionalSocialAccount { get; set; }

        private int GenerateNickCount { get; set; }

        private UserInfo DummyUserInfo = new UserInfo {
         //   CurrentBattleInfo = new BattleInfo(),
            accounts = new Accounts(),
            users = new Users(),
        };

        public static readonly DependencyProperty CurrentSelectedUserProperty =
            DependencyProperty.Register(
                "CurrentSelectedUser",
                typeof(UserInfo),
                typeof(HomePage),
                new PropertyMetadata(null, OnCurrentSelectedUserPropertyChanged));



        public UserInfo CurrentSelectedUser
        {
            get { return GetValue(CurrentSelectedUserProperty) as UserInfo; }
            set { SetValue(CurrentSelectedUserProperty, value); }
        }

        private static void OnCurrentSelectedUserPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pThis = d as HomePage;
            var itemSource = e.NewValue as UserInfo;
            if (itemSource != null)
            {

                Servicecs.Utility.Log($"Select {itemSource.accounts.userId}");
            }
        }


        private async void ToggleSwitch_ToggleChanged(object sender, RoutedEventArgs e)
        {
            var currentUser = CurrentSelectedUser;
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                CurrentSelectedUser = DummyUserInfo;
                this.Bindings.Update();

            });

           await  DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                CurrentSelectedUser = currentUser;
                this.Bindings.Update();
            });

        }
    }
}
