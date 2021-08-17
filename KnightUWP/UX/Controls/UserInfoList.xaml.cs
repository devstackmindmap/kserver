using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 사용자 정의 컨트롤 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=234236에 나와 있습니다.

namespace KnightUWP.UX.Controls
{

    public sealed partial class UserInfoList : UserControl
    {
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(
                "DataSource",
                typeof(ObservableCollection<Accounts>),
                typeof(UserInfoList),
                new PropertyMetadata(null, OnDataSourcePropertyChanged));


        public UserInfoList()
        {
            this.InitializeComponent();

        }

        public ObservableCollection<Accounts> DataSource {
            get { return GetValue(DataSourceProperty) as ObservableCollection<Accounts>; }
            set { SetValue(DataSourceProperty, value); }
        }
        internal void Get()
        {
            var count = DataSource.Count;
            System.Console.WriteLine(count);
        }


        private static void OnDataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pThis = d as UserInfoList;
            var itemSource = e.NewValue as ObservableCollection<Accounts>;
            if (itemSource != null )
            {
                pThis.UserInfoListView.ItemsSource = itemSource;
            }
        }

        private void UserInfoListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            var itemContainer = args.ItemContainer as SelectorItem;

            if (args.ItemIndex % 2 == 0)
            {
                itemContainer.Background = new SolidColorBrush (Colors.AliceBlue);
            }
            else
            {
                itemContainer.Background = null;
            }

        }
    }
}
