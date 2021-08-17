using AkaConfig;
using AkaDB;
using AkaEnum;
using AkaThreading;
using CsvHelper.Configuration;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using AkaUtility;
using KnightUWP.Dao.Text;

namespace KnightUWP.Servicecs
{
    public class DataSetter
    {
        public void DataSet(List<AkaData.ProtoFileInfo> fileList)
        {
            new AkaData.DataSetter().DataSet(fileList);

            foreach (var fileInfo in fileList)
            {
                try
                {
                    var textType = ParseTextType(fileInfo.Name);
                    if (textType != null)
                        TextSet(textType.Value, fileInfo.Url);
                }
                catch (Exception e)
                {
                    throw new Exception("Name: " + fileInfo.Name + ", Url: " + fileInfo.Url);
                }
            }
        }

        private TextType? ParseTextType(string fileName)
        {
            var isSuccess = Enum.TryParse<TextType>(fileName, out var dataType);
            if (!isSuccess)
                return null;

            return dataType;
        }


        private void TextSet(TextType dataType, string url)
        {
            switch (dataType)
            {
                case TextType.text_card_kr:
                    RegisterTableAsDict<uint, TextCardKr>(url, dataType, "TextCardId");
                    break;

            }
        }

        private void RegisterTableAsDict<TKey, TValue>(string url, TextType textType, string tableColumnKeyName)
            where TKey : IComparable
        {

            var bytes = DownloadCsv(url);
            var datas = bytes.ConvertCsvToList<TValue>();

            AddTableAsDict<TKey, TValue>(datas, textType, tableColumnKeyName);
        }

        private void AddTableAsDict<TKey, TValue>(IEnumerable<TValue> datas, TextType textType, string tableColumnKeyName)
            where TKey : IComparable
        {

            var storeMap = MakeStoreDataMap<TKey, TValue>();

            foreach (var data in datas)
            {

                var realKey = MakeTableKey<TKey, TValue>(data, tableColumnKeyName);

                if (!storeMap.ContainsKey(realKey))
                {
                    storeMap.Add(realKey, data);
                }
            }

            TextGetter.AddGameData(textType, storeMap);
        }




        private byte[] DownloadCsv(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var csvBytes = webClient.DownloadData(url);
                return csvBytes;
            }
        }

        private string DownloadJson(string url)
        {
            using (var webClient = new System.Net.WebClient())
            {
                var jsonData = webClient.DownloadString(url);

                return jsonData;
            }
        }


        private SortedDictionary<TKey, TValue> MakeStoreDataMap<TKey, TValue>()
            where TKey : IComparable
        {

            var storeMap = new SortedDictionary<TKey, TValue>(new CustomDataComparerWithSameKey<TKey>());

            return storeMap;
        }


        private TKey MakeTableKey<TKey, TValue>(TValue data, string tableColumnKeyName)
            where TKey : IComparable
        {

            var key = (TKey)GetKeyUsingPropertyName<TValue>(data, tableColumnKeyName);


            return key;
        }

        private object GetKeyUsingPropertyName<T>(T src, string keyName)
        {
            var property = src.GetType().GetProperty(keyName);
            var key = property.GetValue(src, null);

            return key;
        }
    }

    public class TableKey<T1, T2>
        where T1 : IComparable
        where T2 : IComparable
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public TableKey(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TableKey<T1, T2>;

            return other.Item1.Equals(this.Item1) && other.Item2.Equals(this.Item2);
        }
    }

    public class TableKey<T1, T2, T3>
        where T1 : IComparable
        where T2 : IComparable
        where T3 : IComparable
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public T3 Item3 { get; set; }

        public TableKey(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as TableKey<T1, T2, T3>;

            return other.Item1.Equals(this.Item1) && other.Item2.Equals(this.Item2) && other.Item3.Equals(this.Item3);
        }
    }

    public class CustomDataComparerWithDifferentKey<TKey1, TKey2> : IComparer<TableKey<TKey1, TKey2>>
        where TKey1 : IComparable
        where TKey2 : IComparable
    {
        public int Compare(TableKey<TKey1, TKey2> x, TableKey<TKey1, TKey2> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
            }

            return 0;
        }
    }

    public class CustomDataComparerWithDifferentKey<TKey1, TKey2, TKey3> : IComparer<TableKey<TKey1, TKey2, TKey3>>
        where TKey1 : IComparable
        where TKey2 : IComparable
        where TKey3 : IComparable
    {
        public int Compare(TableKey<TKey1, TKey2, TKey3> x, TableKey<TKey1, TKey2, TKey3> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
                else if (result2 == 0)
                {
                    var result3 = x.Item3.CompareTo(y.Item3);
                    if (result3 > 0)
                    {
                        return 1;
                    }
                    else if (result3 < 0)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }
    }

    public class CustomDataComparerWithSameKey<KeyType> : IComparer<TableKey<KeyType, KeyType>>, IComparer<TableKey<KeyType, KeyType, KeyType>>, IComparer<KeyType>
        where KeyType : IComparable
    {
        public int Compare(TableKey<KeyType, KeyType, KeyType> x, TableKey<KeyType, KeyType, KeyType> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
                else if (result2 == 0)
                {
                    var result3 = x.Item3.CompareTo(y.Item3);
                    if (result3 > 0)
                    {
                        return 1;
                    }
                    else if (result3 < 0)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        public int Compare(TableKey<KeyType, KeyType> x, TableKey<KeyType, KeyType> y)
        {
            var result1 = x.Item1.CompareTo(y.Item1);
            if (result1 > 0)
            {
                return 1;
            }
            else if (result1 < 0)
            {
                return -1;
            }
            else if (result1 == 0)
            {
                var result2 = x.Item2.CompareTo(y.Item2);
                if (result2 > 0)
                {
                    return 1;
                }
                else if (result2 < 0)
                {
                    return -1;
                }
            }

            return 0;
        }

        public int Compare(KeyType x, KeyType y)
        {
            var result = x.CompareTo(y);
            if (result > 0)
            {
                return 1;
            }
            else if (result < 0)
            {
                return -1;
            }

            return 0;
        }
    }
}
