using System;
using System.Collections.Generic;

namespace KnightUWP.Dao.Text
{
    public enum TextType
    {
        text_card_kr,
    }


    public class TextGetter
    {
        public static readonly Dictionary<TextType, object> StoreDataMap = new Dictionary<TextType, object>();


        internal static void AddGameData<TKey, TValue>(TextType textType, SortedDictionary<TKey, TValue> storeMap) where TKey : IComparable
        {
            StoreDataMap.Add(textType, storeMap);
        }

        public static string GetCardText(uint cardId)
        {
            var dic = StoreDataMap[TextType.text_card_kr] as SortedDictionary<uint, TextCardKr>;
            TextCardKr text = null;
            if (dic?.TryGetValue(cardId, out  text) ?? false)
            {
                return text.Name;
            }
            return $"Unknown({cardId})";
        }
    }
}
