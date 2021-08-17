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
    public class MessageTypeToStringConverter : IValueConverter
    {


        static MessageTypeToStringConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value.ToString();
        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
