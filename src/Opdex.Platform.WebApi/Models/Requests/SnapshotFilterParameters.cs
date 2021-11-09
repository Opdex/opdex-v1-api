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
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// End time for which to retrieve snapshots.
        /// </summary>
        public DateTime EndDateTime { get; set; }

        /// <inheritdoc />
        protected override SnapshotCursor InternalBuildCursor()
        {
            if (Cursor is null) return new SnapshotCursor(Interval, StartDateTime, EndDateTime, Direction, Limit, PagingDirection.Forward, default);
            Cursor.TryBase64Decode(out var decodedCursor);
            SnapshotCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
