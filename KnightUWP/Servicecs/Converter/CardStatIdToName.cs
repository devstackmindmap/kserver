using AkaData;
using AkaDB.MySql;
using KnightUWP.Dao;
using KnightUWP.Dao.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace KnightUWP.Servicecs
{
    public class CardStatIdToName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cardStatId = (uint)value;

            if (cardStatId == 0)
                return "카드없음";
            var cardStat = TextGetter.GetCardText(cardStatId);
            var card = Data.GetCardStat(cardStatId);
            return $"{cardStat}\n({cardStatId.ToString()}:{card.Elixir})";

        }

        // No need to implement converting back on a one-way binding
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
