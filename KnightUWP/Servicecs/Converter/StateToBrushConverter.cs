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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace KnightUWP.Servicecs
{
    public class StateToBrushConverter : IValueConverter
    {
        static readonly Dictionary<UserState, Brush> BrushTable;


        static StateToBrushConverter()
        {
            BrushTable = new Dictionary<UserState, Brush>();
            BrushTable.Add(UserState.None, (Brush)Application.Current.Resources["Brush-Battle_State_None"]);
            BrushTable.Add(UserState.Matching, (Brush)Application.Current.Resources["Brush-Battle_State_Matching"]);
            BrushTable.Add(UserState.Matched, (Brush)Application.Current.Resources["Brush-Battle_State_Matched"]);
            BrushTable.Add(UserState.BattleEnterRoom, (Brush)Application.Current.Resources["Brush-Battle_State_EnterRoom"]);
            BrushTable.Add(UserState.BattleBeforeStart, (Brush)Application.Current.Resources["Brush-Battle_State_BeforeStart"]);
            BrushTable.Add(UserState.BattleStart, (Brush)Application.Current.Resources["Brush-Battle_State_Start"]);
            BrushTable.Add(UserState.BattleWrongClosed, (Brush)Application.Current.Resources["Brush-Battle_Wrong_Closed"]);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userState = UserState.None;
            if (value is UserState)
                userState = (UserState)value;

            return BrushTable[userState];
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
