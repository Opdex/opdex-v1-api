using System;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots
{
    /// <summary>
    /// Retrieve a single token snapshot of specified type. Returns a matching record by dateTime and type from the database else the most
    /// recent record by type. If nothing is found, will return a new snapshot instance.
    /// </summary>
    public class RetrieveTokenSnapshotWithFilterQuery : IRequest<TokenSnapshot>
    {
        /// <summary>
        /// Create the retrieve token snapshot with filter query.
        /// </summary>
        /// <param name="tokenId">The token id of the snapshot to find.</param>
        /// <param name="marketId">The market id of the snapshot.</param>
        /// <param name="dateTime">The requested datetime of the snapshot.</param>
        /// <param name="snapshotType">The type of snapshot.</param>
        public RetrieveTokenSnapshotWithFilterQuery(long tokenId, long marketId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId), $"{nameof(tokenId)} must be greater than 0.");
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            TokenId = tokenId;
            MarketId = marketId;
            DateTime = dateTime;
            SnapshotType = snapshotType;
        }

        public long TokenId { get; }
        public long MarketId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}
