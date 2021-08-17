using AkaEnum;
using System;
using System.Collections.Generic;

namespace AkaUtility
{
    public static class StringExtension
    {
        public static T CastToEnum<T>(this string data)
        {
            return (T)System.Enum.Parse(typeof(T), data);
        }

        public static TOut[] CastToArray<TOut>(this string datas, Converter<string, TOut> converter)
        {
            if (datas != "" && datas != string.Empty)
            {
                var splited_list = datas.Split(new char[] { '/' });
                return Array.ConvertAll(splited_list, converter);
            }
            else
            {
                return new TOut[] { };
            }
        }

        public static IDictionary<TKey, TValue> CastToDictionary<TKey, TValue>(this string texts)
        {
            var dict = new Dictionary<TKey, TValue>();
            var elements = texts.Split('/');
            foreach (var element in elements)
            {
                var datas = element.Split(',');
                var key = (TKey)Convert.ChangeType(datas[0], typeof(TKey));
                var value = (TValue)Convert.ChangeType(datas[1], typeof(TValue));

                dict.Add(key, value);
            }

            return dict;
        }

        public static List<TOut> CastToList<TOut>(this string datas, Converter<string, TOut> converter)
        {
            var convertedList = new List<TOut>();

            if (datas != "" && datas != string.Empty)
            {
                var splited_list = datas.Split(new char[] { '/' });

                foreach (var ele in splited_list)
                {
                    var value = converter(ele);

                    convertedList.Add(converter(ele));
                }
            }

            return convertedList;
        }

        public static string FormatWith(this string text, params object[] args)
        {
            return String.Format(text, args);
        }

        public static string GetFileExtensionName(this string url)
        {
            var startIndex = url.LastIndexOf('.') + 1;

            return url.Substring(startIndex);
        }

        public static FileExtensionType ParseFileExtensionType(this string text)
        {
            return (FileExtensionType)Enum.Parse(typeof(FileExtensionType), text);
        }

        public static int ToInt(this string t)
        {
            return int.TryParse(t, out var v) ? v : 0;
        }

        public static float ToFloat(this string t)
        {
            return float.TryParse(t, out var v) ? v : 0f;
        }
    }
}
