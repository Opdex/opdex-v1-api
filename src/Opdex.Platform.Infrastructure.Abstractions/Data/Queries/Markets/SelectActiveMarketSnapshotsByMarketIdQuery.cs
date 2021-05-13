using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectActiveMarketSnapshotsByMarketIdQuery : IRequest<IEnumerable<MarketSnapshot>>
    {
        public SelectActiveMarketSnapshotsByMarketIdQuery(long marketId, DateTime time)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            MarketId = marketId;
            Time = time;
        }
        
        public long MarketId { get; }
        public DateTime Time { get; }
    }
}