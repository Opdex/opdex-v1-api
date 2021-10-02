using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots
{
    /// <summary>
    /// Retrieve a single market snapshot of specified type. Returns a matching record by dateTime and type from the database else the most
    /// recent record by type. If nothing is found, will return a new snapshot instance.
    /// </summary>
    public class RetrieveMarketSnapshotWithFilterQuery : IRequest<MarketSnapshot>
    {
        /// <summary>
        /// Create the retrieve market snapshot with filter query.
        /// </summary>
        /// <param name="marketId">The market id to get a snapshot of.</param>
        /// <param name="dateTime">The datetime to get the snapshot of.</param>
        /// <param name="snapshotType">The snapshot type - currently only daily is enabled.</param>
        public RetrieveMarketSnapshotWithFilterQuery(long marketId, DateTime dateTime, SnapshotType snapshotType)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            MarketId = marketId;
            DateTime = dateTime;
            SnapshotType = snapshotType;
        }

        public long MarketId { get; }
        public DateTime DateTime { get; }
        public SnapshotType SnapshotType { get; }
    }
}



