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
    public class StateToColorConverter : IValueConverter
    {
        static readonly Dictionary<UserState, Color> ColorTable;


        static StateToColorConverter()
        {
            ColorTable = new Dictionary<UserState, Color>();
            ColorTable.Add(UserState.None, (Color)Application.Current.Resources["Color_Battle_State_None"]);
            ColorTable.Add(UserState.Matching, (Color)Application.Current.Resources["Color_Battle_State_Matching"]);
            ColorTable.Add(UserState.Matched, (Color)Application.Current.Resources["Color_Battle_State_Matched"]);
            ColorTable.Add(UserState.BattleEnterRoom, (Color)Application.Current.Resources["Color_Battle_State_EnterRoom"]);
            ColorTable.Add(UserState.BattleBeforeStart, (Color)Application.Current.Resources["Color_Battle_State_BeforeStart"]);
            ColorTable.Add(UserState.BattleStart, (Color)Application.Current.Resources["Color_Battle_State_Start"]);
            ColorTable.Add(UserState.BattleWrongClosed, (Color)Application.Current.Resources["Color_Battle_Wrong_Closed"]);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userState = UserState.None;
            if (value is UserState)
                userState = (UserState)value;

            return ColorTable[userState];
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
