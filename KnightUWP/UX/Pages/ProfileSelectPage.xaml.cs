using Dwrandaz.AutoUpdateComponent;
using KnightUWP.Servicecs;
using KnightUWP.UX.Frame;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace KnightUWP.UX.Pages
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class ProfileSelectPage
    {

        public ProfileSelectPage()
        {
            this.InitializeComponent();
            profileList.ItemsSource = ProfileManager.Profiles;


        }

        private void profileList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProfileManager.Load(e.ClickedItem);
            
            Windows.UI.Xaml.Controls.Frame rootFrame = Window.Current.Content as Windows.UI.Xaml.Controls.Frame;
            rootFrame.Navigate(typeof(RootFrame), null, new DrillInNavigationTransitionInfo());
            rootFrame.BackStack.Clear();
        }
    }
}
