using System;

namespace Opdex.Platform.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUnixTimeSeconds(this string seconds)
        {
            var success = long.TryParse(seconds, out var timeSeconds);
            return !success ? default : DateTimeOffset.FromUnixTimeSeconds(timeSeconds).UtcDateTime;
        }

        public static DateTime StartOfMonth(this DateTime date)
        {
            throw new NotImplementedException();
        }

        public static DateTime EndOfMonth(this DateTime date)
        {
            throw new NotImplementedException();
        }

        public static DateTime StartOfWeek(this DateTime date)
        {
            throw new NotImplementedException();
        }

        public static DateTime EndOfWeek(this DateTime date)
        {
            throw new NotImplementedException();
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return date.EndOfHour().AddHours(23 - date.Hour);
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
            return date.EndOfMinute().AddMinutes(59 - date.Minute);
        }

        public static DateTime StartOfMinute(this DateTime date)
        {
            return date.AddSeconds(-date.Second).AddMilliseconds(-date.Millisecond);
        }

        public static DateTime EndOfMinute(this DateTime date)
        {
            return date.AddSeconds(59 - date.Second);
        }

        public static DateTime ToStartOf(this DateTime date, SnapshotType snapshotType)
        {
            switch (snapshotType)
            {
                case SnapshotType.Minute:
                    return date.StartOfMinute();
                case SnapshotType.Hourly:
                    return date.StartOfHour();
                case SnapshotType.Daily:
                    return date.StartOfDay();
                default:
                    throw new ArgumentOutOfRangeException(nameof(snapshotType), snapshotType, "Invalid snapshot type.");
            }
        }

        public static DateTime ToEndOf(this DateTime date, SnapshotType snapshotType)
        {
            switch (snapshotType)
            {
                case SnapshotType.Minute:
                    return date.EndOfMinute();
                case SnapshotType.Hourly:
                    return date.EndOfHour();
                case SnapshotType.Daily:
                    return date.EndOfDay();
                default:
                    throw new ArgumentOutOfRangeException(nameof(snapshotType), snapshotType, "Invalid snapshot type.");
            }
        }
    }
}
