using System;

namespace Opdex.Platform.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUnixTimeSeconds(this string seconds)
        {
            long.TryParse(seconds, out var timeSeconds);
            return DateTimeOffset.FromUnixTimeSeconds(timeSeconds).UtcDateTime;
        }
        
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime();
        }
        
        public static DateTime EndOfMonth(this DateTime date)
        {
            return new DateTime();
        }

        public static DateTime StartOfWeek(this DateTime date)
        {
            return new DateTime();
        }
        
        public static DateTime EndOfWeek(this DateTime date)
        {
            return new DateTime();
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.EndOfHour().AddHours(24 - date.Hour);
        }
        
        public static DateTime StartOfDay(this DateTime date)
        {
            return date.StartOfHour().AddHours(-date.Hour);
        }
        
        public static DateTime StartOfHour(this DateTime date)
        {
            return date.StartOfMinute().AddMinutes(-date.Minute);
        }
        
        public static DateTime EndOfHour(this DateTime date)
        {
            return date.EndOfMinute().AddMinutes(60 - date.Minute);
        }
        
        public static DateTime StartOfMinute(this DateTime date)
        {
            return date.AddSeconds(-date.Second);
        }
        
        public static DateTime EndOfMinute(this DateTime date)
        {
            return date.AddSeconds(60 - date.Second);
        }
    }
}