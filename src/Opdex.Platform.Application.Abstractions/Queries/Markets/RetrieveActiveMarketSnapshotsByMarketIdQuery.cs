using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveActiveMarketSnapshotsByMarketIdQuery : IRequest<IEnumerable<MarketSnapshot>>
    {
        public RetrieveActiveMarketSnapshotsByMarketIdQuery(long marketId, DateTime time)
        {
            MarketId = marketId;
            Time = time;
        }
        
        public long MarketId { get; }
        public DateTime Time { get; }
    }
}