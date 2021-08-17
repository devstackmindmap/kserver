using System;

namespace AkaUtility
{
    public static class DateTimeExtension
    {
        public static string ToTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static DateTime MinusDays(this DateTime dateTime, int days)
        {
            DateTime d = dateTime.AddDays(-days);
            return d;
        }

        public static DateTime MinusMinutes(this DateTime dateTime, double minutes)
        {
            DateTime d = dateTime.AddMinutes(-minutes);
            return d;
        }
    }
}
