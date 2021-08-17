using AkaDB.MySql;
using CommonProtocol;
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
    public class MessageTypeToColorConverter : IValueConverter
    {
        static readonly Dictionary<MessageType, Color> ColorTable;


        static MessageTypeToColorConverter()
        {
            ColorTable = new Dictionary<MessageType, Color>();
            ColorTable.Add(MessageType.None, (Color)Application.Current.Resources["Color_Battle_Action_Default"]);
            ColorTable.Add(MessageType.AttackUnit, (Color)Application.Current.Resources["Color_Battle_Action_Attack"]);
            ColorTable.Add(MessageType.Skill, (Color)Application.Current.Resources["Color_Battle_Action_Skill"]);
            ColorTable.Add(MessageType.CardUseResult, (Color)Application.Current.Resources["Color_Battle_State_Action_CardUse"]);
            ColorTable.Add(MessageType.UnitDeath, (Color)Application.Current.Resources["Color_Battle_State_UnitDeath"]);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var userState = MessageType.None;
            if (value is MessageType)
                userState = (MessageType)value;
            if (ColorTable.ContainsKey(userState) == false)
                userState = MessageType.None;

            return ColorTable[userState];
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
