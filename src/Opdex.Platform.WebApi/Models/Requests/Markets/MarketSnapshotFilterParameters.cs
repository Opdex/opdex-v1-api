using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.WebApi.Models.Requests.Markets
{
    public class MarketSnapshotFilterParameters : FilterParameters<SnapshotCursor>
    {
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
            if (Cursor is null) return new SnapshotCursor(Interval.OneDay, StartDateTime, EndDateTime, Direction, Limit, PagingDirection.Forward, default);
            Base64Extensions.TryBase64Decode(Cursor, out var decodedCursor);
            SnapshotCursor.TryParse(decodedCursor, out var cursor);
            return cursor;
        }
    }
}
