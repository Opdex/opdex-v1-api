using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots
{
    /// <summary>
    /// Selects token snapshots for a given token.
    /// </summary>
    public class SelectTokenSnapshotsWithFilterQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        /// <summary>
        /// Creates a request to select snapshot data for a given token.
        /// </summary>
        /// <param name="tokenId">The token id of snapshots to find.</param>
        /// <param name="marketId">The market id of snapshots to find, or 0 for global snapshots.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public SelectTokenSnapshotsWithFilterQuery(ulong tokenId, ulong marketId, SnapshotCursor cursor)
        {
            MarketId = marketId;
            TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId));
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
        }

        public ulong TokenId { get; }
        public ulong MarketId { get; }
        public SnapshotCursor Cursor { get; }
    }
}
