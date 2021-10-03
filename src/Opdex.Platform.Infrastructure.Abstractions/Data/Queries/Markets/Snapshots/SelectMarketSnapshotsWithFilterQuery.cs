using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots
{
    /// <summary>
    /// Select market snapshots by the associated market id, a date range and snapshot type.
    /// </summary>
    public class SelectMarketSnapshotsWithFilterQuery : IRequest<IEnumerable<MarketSnapshot>>
    {
        /// <summary>
        /// Constructor to create the select market snapshots with filter query.
        /// </summary>
        /// <param name="marketId">The market id of snapshots to find.</param>
        /// <param name="startDate">The start date, earliest snapshot to find.</param>
        /// <param name="endDate">The end date, latest snapshot to find.</param>
        /// <param name="snapshotType">The type of snapshots to return, hourly/daily options.</param>
        public SelectMarketSnapshotsWithFilterQuery(ulong marketId, DateTime startDate, DateTime endDate, SnapshotType snapshotType)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            if (!snapshotType.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(snapshotType));
            }

            MarketId = marketId;
            StartDate = startDate;
            EndDate = endDate;
            SnapshotType = snapshotType;
        }

        public ulong MarketId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public SnapshotType SnapshotType { get; }
    }
}
