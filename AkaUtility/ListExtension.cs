using System.Collections.Generic;

namespace AkaUtility
{
    public static class ListExtension
    {
        public static void AddList<T, T2>(this List<T> data, Dictionary<T2, T> addData)
        {
            data.AddRange(addData.Values);
        }

        public static T LastOrDefaultEx<T>(this List<T> data) where T : class
        {
            return data.Count > 0 ? data[data.Count - 1] : null;
        }

        public static T LastEx<T>(this List<T> data) 
        {
            return data[data.Count - 1];
        }
    }
}
