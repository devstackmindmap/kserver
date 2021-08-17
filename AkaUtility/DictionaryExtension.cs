using System.Collections.Generic;

namespace AkaUtility
{
    public static class DictionaryExtension
    {
        public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> data, TKey key)
        {
            return data.TryGetValue(key, out var result) ? result : default(TValue);
        }
    }
}
