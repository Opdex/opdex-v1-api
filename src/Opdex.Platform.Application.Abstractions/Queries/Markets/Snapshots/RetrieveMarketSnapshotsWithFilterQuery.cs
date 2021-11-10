using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots
{
    /// <summary>
    /// Retrieve market snapshots by the associated market id.
    /// </summary>
    public class RetrieveMarketSnapshotsWithFilterQuery : IRequest<IEnumerable<MarketSnapshot>>
    {
        /// <summary>
        /// Constructor to create the retrieve market snapshots with filter query.
        /// </summary>
        /// <param name="marketId">The market id of snapshots to find.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public RetrieveMarketSnapshotsWithFilterQuery(ulong marketId, SnapshotCursor cursor)
        {
            MarketId = marketId > 0 ? marketId : throw new ArgumentOutOfRangeException(nameof(marketId));
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public ulong MarketId { get; }
        public SnapshotCursor Cursor { get; }
    }
}
