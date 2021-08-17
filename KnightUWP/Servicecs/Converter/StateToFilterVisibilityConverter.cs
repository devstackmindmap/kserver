using AkaDB.MySql;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace KnightUWP.Servicecs
{
    public class StateToFilterVisibilityConverter : IValueConverter
    {
        static readonly Dictionary<UserState, Visibility> VisibilityTable;


        static StateToFilterVisibilityConverter()
        {
            VisibilityTable = new Dictionary<UserState, Visibility>();
            VisibilityTable.Add(UserState.None, Visibility.Visible);
            VisibilityTable.Add(UserState.Matching, Visibility.Collapsed);
            VisibilityTable.Add(UserState.Matched, Visibility.Collapsed);
            VisibilityTable.Add(UserState.BattleEnterRoom, Visibility.Visible);
            VisibilityTable.Add(UserState.BattleBeforeStart, Visibility.Visible);
            VisibilityTable.Add(UserState.BattleStart, Visibility.Collapsed);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userState = UserState.None;
            if (value is UserState)
                userState = (UserState)value;

            return VisibilityTable[userState];
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
