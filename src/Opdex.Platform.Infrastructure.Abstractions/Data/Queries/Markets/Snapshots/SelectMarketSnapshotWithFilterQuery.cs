using MediatR;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets.Snapshots
{
    public class SelectMarketSnapshotWithFilterQuery : IRequest<MarketSnapshot>
    {
        public SelectMarketSnapshotWithFilterQuery(long marketId, DateTime date, SnapshotType snapshotType)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId), "marketId must be greater than 0.");
            }

            MarketId = marketId;
            Date = date;
            SnapshotType = snapshotType;
        }

        public long MarketId { get; }
        public DateTime Date { get; }
        public SnapshotType SnapshotType { get; }
    }
}
