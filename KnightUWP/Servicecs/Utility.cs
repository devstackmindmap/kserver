using AkaDB.MySql;
using KnightUWP.Dao;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace KnightUWP.Servicecs
{
    public static class Utility
    {
        public static string Format(string formatString, params string[] datas)
        {
            return String.Format(formatString, datas);
        }

        public static DateTime ToDateTime(long tick)
        {
            return new DateTime(tick);
        }


        static LoggingChannel _channel = new LoggingChannel("Knightest UWP app", new LoggingChannelOptions(), new Guid("F5A63118-80D4-4373-A6E4-101F3EA0782F"));

        public static void Log(string log)
        {
            _channel.LogMessage(log);
        }


        static long ticks = new DateTime(2019, 1, 1).Ticks;
        public static string UniqueId()
        {
            var ans = DateTime.Now.Ticks - ticks;
            return ans.ToString("x");
        }
    }
}
