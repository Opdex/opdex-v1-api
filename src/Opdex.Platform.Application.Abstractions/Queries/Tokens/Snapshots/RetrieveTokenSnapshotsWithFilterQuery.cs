using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots
{
    /// <summary>
    /// Retrieves token snapshots for a given token.
    /// </summary>
    public class RetrieveTokenSnapshotsWithFilterQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        /// <summary>
        /// Creates a request to retrieve snapshot data for a given token.
        /// </summary>
        /// <param name="tokenId">The token id of snapshots to find.</param>
        /// <param name="marketId">The market id of snapshots to find, or 0 for global snapshots.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public RetrieveTokenSnapshotsWithFilterQuery(ulong tokenId, ulong marketId, SnapshotCursor cursor)
        {
            if (tokenId < 1) throw new ArgumentOutOfRangeException(nameof(tokenId));
            if (cursor is null) throw new ArgumentNullException(nameof(cursor));

            TokenId = tokenId;
            MarketId = marketId;
            Cursor = cursor;
        }

        public ulong TokenId { get; }
        public ulong MarketId { get; }
        public SnapshotCursor Cursor { get; }
    }
}
