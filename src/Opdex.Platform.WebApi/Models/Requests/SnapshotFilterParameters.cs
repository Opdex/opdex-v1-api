using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.WebApi.Models.Requests
{
    public class SnapshotFilterParameters : FilterParameters<SnapshotCursor>
    {
        /// <summary>
        /// The snapshot step interval.
        /// </summary>
        public Interval Interval { get; set; }

        /// <summary>
        /// Start time for which to retrieve snapshots.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// End time for which to retrieve snapshots.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <inheritdoc />
        protected override SnapshotCursor InternalBuildCursor()
        {
            if (Cursor is null) return new SnapshotCursor(Interval, StartTime, EndTime, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            SnapshotCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
