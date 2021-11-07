using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries
{
    /// <summary>
    /// Used with historical data queries, so that snapshots can be retrieved in a paginated manor.
    /// </summary>
    public class SnapshotCursor : Cursor<(DateTime, ulong)>
    {
        // greater than hours in a month
        public const uint MaxLimit = 750;

        public SnapshotCursor(Interval interval, DateTime startTime, DateTime endTime, SortDirectionType sortDirection,
                              uint limit, PagingDirection pagingDirection, (DateTime, ulong) pointer)
            : base(sortDirection, limit, pagingDirection, pointer, DefaultLimitFromInterval(interval), MaxLimit, SortDirectionType.ASC)
        {
            Interval = interval;
            StartTime = startTime;
            EndTime = endTime;
        }

        public Interval Interval { get; }
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            var pointerBytes = Encoding.UTF8.GetBytes($"({new DateTimeOffset(Pointer.Item1).ToUnixTimeSeconds()}, {Pointer.Item2})");
            var encodedPointer = Convert.ToBase64String(pointerBytes);

            var sb = new StringBuilder();
            sb.AppendFormat("direction:{0};limit:{1};paging:{2};", SortDirection, Limit, PagingDirection);
            sb.AppendFormat("interval:{0};", Interval);
            sb.AppendFormat("start:{0};", new DateTimeOffset(StartTime).ToUnixTimeSeconds());
            sb.AppendFormat("end:{0};", new DateTimeOffset(EndTime).ToUnixTimeSeconds());
            sb.AppendFormat("pointer:{0};", encodedPointer);
            return sb.ToString();
        }

        /// <inheritdoc />
        public override Cursor<(DateTime, ulong)> Turn(PagingDirection direction, (DateTime, ulong) pointer)
        {
            if (!direction.IsValid()) throw new ArgumentOutOfRangeException(nameof(direction), "Invalid paging direction.");
            if (pointer == Pointer) throw new ArgumentOutOfRangeException(nameof(pointer), "Cannot paginate with an identical pointer.");

            return new SnapshotCursor(Interval, StartTime, EndTime, SortDirection, Limit, direction, pointer);
        }

        /// <summary>
        /// Parses a stringified version of the cursor
        /// </summary>
        /// <param name="raw">Stringified cursor</param>
        /// <param name="cursor">Parsed cursor</param>
        /// <returns>True if the value could be parsed, otherwise false</returns>
        public static bool TryParse(string raw, out SnapshotCursor cursor)
        {
            cursor = null;

            if (raw is null) return false;

            var values = ToDictionary(raw);

            if (!TryGetCursorProperty<Interval>(values, "interval", out var interval)) return false;

            if (!TryGetCursorProperty<DateTime>(values, "start", out var start)) return false;

            if (!TryGetCursorProperty<DateTime>(values, "end", out var end)) return false;

            if (!TryGetCursorProperty<SortDirectionType>(values, "direction", out var direction)) return false;

            if (!TryGetCursorProperty<uint>(values, "limit", out var limit)) return false;

            if (!TryGetCursorProperty<string>(values, "pointer", out var pointerEncoded)) return false;

            if (!pointerEncoded.HasValue()) return false;

            if (!TryDecodePointer(pointerEncoded, out var pointer)) return false;

            if (!TryGetCursorProperty<PagingDirection>(values, "paging", out var paging)) return false;

            try
            {
                cursor = new SnapshotCursor(interval, start, end, direction, limit, paging, pointer);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private static bool TryDecodePointer(string encoded, out (DateTime, ulong) pointer)
        {
            pointer = (default, default);

            if (!encoded.TryBase64Decode(out var decoded)) return false;

            var tupleParts = decoded.Replace("(", "").Replace(")", "").Split(',');

            if (tupleParts.Length != 2 || !long.TryParse(tupleParts[0], out var unixStartTime) || !ulong.TryParse(tupleParts[1], out var identifier)) return false;

            const long MaxUnixTime = 253402300799;
            if (unixStartTime < 0 || unixStartTime > MaxUnixTime) return false;

            var startTime = DateTimeOffset.FromUnixTimeSeconds(unixStartTime).UtcDateTime;

            pointer = (startTime, identifier);
            return true;
        }

        private static uint DefaultLimitFromInterval(Interval interval)
        {
            return interval switch
            {
                Interval.OneHour => 168, // one week
                Interval.OneDay => 28, // four weeks
                _ => throw new ArgumentOutOfRangeException(nameof(interval))
            };
        }
    }

    /// <summary>
    /// Supported snapshot filter intervals.
    /// </summary>
    [TypeConverter(typeof(EnumMemberConverter<Interval>))]
    [DataContract]
    public enum Interval
    {
        [EnumMember(Value = "1H")]
        OneHour = 0,

        [EnumMember(Value = "1D")]
        OneDay = 1
    }
}
