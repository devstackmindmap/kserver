using KnightUWP.Servicecs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234238에 나와 있습니다.

namespace KnightUWP.UX.Pages
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class UserStatePage : Page
    {
        public UserStatePage()
        {
            this.InitializeComponent();

            this.Loaded += (sender, e) =>
            {
            };

            this.Unloaded += (sender, e) =>
            {
            };

        }


        private void BattleHisotriy_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var itemContainer = args.ItemContainer as SelectorItem;

            if (args.ItemIndex % 2 == 0)
            {
                itemContainer.Background = new SolidColorBrush(Colors.AliceBlue);
            }
            else
            {
                itemContainer.Background = new SolidColorBrush(Colors.White); ;
            }

         //   if (args.ItemIndex == sender.Items.Count -1 )
         //       sender.ScrollIntoView(sender.Items.Last());
        }

        private void BattleHisotriyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
