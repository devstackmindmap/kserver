using KnightUWP.UX.Pages;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace KnightUWP.UX.Frame
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class RootFrame : Page
    {
        public static RootFrame MainFrame { get; private set; }


        public Page CurrentPage => NavigationFrame.Content as Page;

        public RootFrame()
        {
            this.InitializeComponent();
            MainFrame = this;



            this.Loaded += (sender, e) =>
            {
            };

            this.Unloaded += (sender, e) =>
            {
                
            };
        }

        public void GotoNavigate(string targetPageName, params string[] args)
        {
            Type targetPageType = null;
            switch (targetPageName)
            {
                case null:
                case "" :
                    targetPageType = typeof(HomePage);
                    break;
            }
            
            NavigationFrame.Navigate(targetPageType, null, new SuppressNavigationTransitionInfo());
        }


        public void ShowLoading()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                progressControl.Visibility = Visibility.Visible;
            }, Windows.UI.Core.CoreDispatcherPriority.Normal);
        }

        public void HideLoading()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                progressControl.Visibility = Visibility.Collapsed;
            }, Windows.UI.Core.CoreDispatcherPriority.Normal);
        }




        protected override  void OnNavigatedTo(NavigationEventArgs e)
        {
            GotoNavigate(null);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double height = 800;
            double width = 1400;
            if (e.NewSize.Height > height)
                height = e.NewSize.Height;
            if (e.NewSize.Width > width)
                width = e.NewSize.Width;
            var newSize = new Size(width, height);
            if (e.NewSize != newSize)
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryResizeView(newSize);
        }
    }
}
