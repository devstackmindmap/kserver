using AkaEnum;
using AkaUtility;
using System;
using System.Collections.Generic;

namespace AkaData
{
    public static class DataGetter
    {
        public static readonly Dictionary<DataType, object> StoreDataMap = new Dictionary<DataType, object>(DataTypeComparer.Comparer);

        public static IList<T> CastToAsList<T>(DataType dataType)
        {
            if (false == StoreDataMap.ContainsKey(dataType))
                Console.WriteLine($"StoreDataMap not contain {dataType.ToString()} dataType");

            return StoreDataMap[dataType] as IList<T>;
        }

        public static IDictionary<TKey, T> CastToDictionaryAsOneKey<TKey, T>(DataType dataType)
            where TKey : IComparable
        {
            if (false == StoreDataMap.ContainsKey(dataType))
                Console.WriteLine($"StoreDataMap not contain {dataType.ToString()} dataType");

            var data = StoreDataMap[dataType];
            return data as IDictionary<TKey, T>;
        }

        public static IDictionary<TableKey<TKey1, TKey2>, T> CastToDictionaryAsTwoKey<TKey1, TKey2, T>(DataType dataType)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            if (false == StoreDataMap.ContainsKey(dataType))
                Console.WriteLine($"StoreDataMap not contain {dataType.ToString()} dataType");

            return StoreDataMap[dataType] as IDictionary<TableKey<TKey1, TKey2>, T>;
        }

        public static bool ContainsKey<TKey, T>(DataType dataType, TKey key)
            where TKey : IComparable
        {
            return CastToDictionaryAsOneKey<TKey, T>(dataType).ContainsKey(key);
        }

        public static bool ContainsKey<T>(DataType dataType, uint key1, uint key2)
        {
            var key = new TableKey<uint, uint>(key1, key2);

            return CastToDictionaryAsTwoKey<uint, uint, T>(dataType).ContainsKey(key);
        }

        public static void AddGameData(DataType dataType, object data)
        {
            if (StoreDataMap.ContainsKey(dataType))
            {
                StoreDataMap[dataType] = data;
            }
            else
            {
                StoreDataMap.Add(dataType, data);
            }
        }

        public static IList<T> GetGameDataAsList<T>(DataType dataType)
        {
            return CastToAsList<T>(dataType);
        }

        public static T GetGameData<T>(DataType dataType)
            where T : class
        {
            return StoreDataMap[dataType] as T;
        }
        public static IDictionary<TKey, TValue> GetPrimitiveDict<TKey, TValue>(DataType dataType)
            where TKey : IComparable
        {
            return CastToDictionaryAsOneKey<TKey, TValue>(dataType);
        }

        public static ICollection<TValue> GetPrimitiveValues<TKey1, TKey2, TValue>(DataType dataType)
            where TKey1 : IComparable
            where TKey2 : IComparable

        {
            return CastToDictionaryAsTwoKey<TKey1, TKey2, TValue>(dataType).Values;
        }

        public static ICollection<TValue> GetPrimitiveValues<TKey, TValue>(DataType dataType)
            where TKey : IComparable

        {
            return CastToDictionaryAsOneKey<TKey, TValue>(dataType).Values;
        }

        public static IDictionary<TableKey<TKey1, TKey2>, TValue> GetPrimitiveDictWithTwoKey<TKey1, TKey2, TValue>(DataType dataType)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            return CastToDictionaryAsTwoKey<TKey1, TKey2, TValue>(dataType);
        }

        public static IDictionary<TKey, TValue> GetGameData<TKey, TValue>(DataType dataType)
            where TKey : IComparable
        {
            return CastToDictionaryAsOneKey<TKey, TValue>(dataType);
        }

        public static IDictionary<TableKey<TKey1, TKey2>, TValue> GetGameData<TKey1, TKey2, TValue>(DataType dataType)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            return CastToDictionaryAsTwoKey<TKey1, TKey2, TValue>(dataType);
        }

        public static TValue GetGameData<TKey, TValue>(DataType dataType, TKey key)
            where TKey : IComparable
        {
            var dict = CastToDictionaryAsOneKey<TKey, TValue>(dataType);

            if (false == dict.ContainsKey(key))
                Console.WriteLine($"{dataType.ToString()} dataType not Contain {key.ToString()} key");

            return dict.SafeGet(key);
        }

        public static TValue GetGameDataNotErrLog<TKey, TValue>(DataType dataType, TKey key)
            where TKey : IComparable
        {
            var dict = CastToDictionaryAsOneKey<TKey, TValue>(dataType);
            return dict.SafeGet(key);
        }


        public static TValue GetGameData<TKey1, TKey2, TValue>(DataType dataType, TKey1 key1, TKey2 key2)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            var key = new TableKey<TKey1, TKey2>(key1, key2);

            var dict = CastToDictionaryAsTwoKey<TKey1, TKey2, TValue>(dataType);

            if (false == dict.ContainsKey(key))
                Console.WriteLine($"{dataType.ToString()} dataType not Contain {key.ToString()} key");

            return dict.SafeGet(key);
        }

        public static bool IsContainsKey<TKey1, TKey2, TValue>(DataType dataType, TKey1 key1, TKey2 key2)
            where TKey1 : IComparable
            where TKey2 : IComparable
        {
            var key = new TableKey<TKey1, TKey2>(key1, key2);
            var dict = CastToDictionaryAsTwoKey<TKey1, TKey2, TValue>(dataType);
            return dict.ContainsKey(key);
        }
    }
}
