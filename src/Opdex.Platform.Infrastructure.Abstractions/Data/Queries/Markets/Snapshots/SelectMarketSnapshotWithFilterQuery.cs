using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots
{
    /// <summary>
    /// Select a single market snapshot of specified type. Returns a matching record by dateTime and type from the database else the most
    /// recent record by type. If nothing is found, will return a new snapshot instance.
    /// </summary>
    public class SelectMarketSnapshotWithFilterQuery : IRequest<MarketSnapshot>
    {
        /// <summary>
        /// Create the select market snapshot with filter query.
        /// </summary>
        /// <param name="marketId">The market id to get a snapshot of.</param>
        /// <param name="dateTime">The datetime to get the snapshot of.</param>
        /// <param name="snapshotType">The snapshot type - currently only daily is enabled.</param>
        public SelectMarketSnapshotWithFilterQuery(ulong marketId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "marketId must be greater than 0.");
            }

            MarketId = marketId;
            DateTime = dateTime;

            // Todo: Is this needed? We're only storing daily snapshots of markets currently
            SnapshotType = snapshotType;
        }

        public ulong MarketId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}
