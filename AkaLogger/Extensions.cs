using NLog;
using System;

namespace AkaLogger
{   
    internal static class Extensions
    {
        internal static string ToLog(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

    }

}
