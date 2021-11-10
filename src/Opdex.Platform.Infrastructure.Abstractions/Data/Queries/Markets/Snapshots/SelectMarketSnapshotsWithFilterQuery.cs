using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots
{
    /// <summary>
    /// Select market snapshots by the associated market id.
    /// </summary>
    public class SelectMarketSnapshotsWithFilterQuery : IRequest<IEnumerable<MarketSnapshot>>
    {
        /// <summary>
        /// Constructor to create the select market snapshots with filter query.
        /// </summary>
        /// <param name="marketId">The market id of snapshots to find.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public SelectMarketSnapshotsWithFilterQuery(ulong marketId, SnapshotCursor cursor)
        {
            MarketId = marketId > 0 ? marketId : throw new ArgumentOutOfRangeException(nameof(marketId));
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public ulong MarketId { get; }
        public SnapshotCursor Cursor { get; }
    }
}
