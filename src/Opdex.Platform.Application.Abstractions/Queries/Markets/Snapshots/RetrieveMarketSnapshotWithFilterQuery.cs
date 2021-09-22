using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets.Snapshots
{
    public class RetrieveMarketSnapshotWithFilterQuery : IRequest<MarketSnapshot>
    {
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



