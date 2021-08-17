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

namespace KnightUWP.Servicecs
{
    public class CardColorConverter : IValueConverter
    {
        static readonly Color nextCard;
        static readonly Color normalCard;

        static CardColorConverter()
        {
            nextCard = (Color)Application.Current.Resources["Color_Battle_Action_Skill"];
            normalCard = (Color)Application.Current.Resources["Color_Battle_Action_Default"];
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var myIndex = parameter.ToString();
            var selectedIndex = value.ToString();
            return myIndex == selectedIndex ? nextCard : normalCard;
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
