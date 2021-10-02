using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots
{
    /// <summary>
    /// Retrieve token snapshots by a token and its market, their type and date range else an empty list.
    /// </summary>
    public class RetrieveTokenSnapshotsWithFilterQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        /// <summary>
        /// Create the retrieve token snapshots with filter query.
        /// </summary>
        /// <param name="tokenId">The token id of snapshots to find.</param>
        /// <param name="marketId">The market id of snapshots to find.</param>
        /// <param name="startDate">The start date, earliest snapshot to find.</param>
        /// <param name="endDate">The end date, latest snapshot to find.</param>
        /// <param name="snapshotType">The type of snapshots requested, CRS is minute/hourly/daily, SRC is hourly/daily.</param>
        public RetrieveTokenSnapshotsWithFilterQuery(long tokenId, long marketId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            // 0 is valid
            if (marketId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "Market id must be greater or equal to zero.");
            }

            if (startDate.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(startDate));
            }

            if (endDate.Equals(default) || endDate < startDate)
            {
                throw new ArgumentOutOfRangeException(nameof(endDate));
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            TokenId = tokenId;
            MarketId = marketId;
            StartDate = startDate;
            EndDate = endDate;
            SnapshotType = snapshotType;
        }

        public long TokenId { get; }
        public long MarketId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
