using System;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectActiveMarketRouterByMarketIdQuery : FindQuery<MarketRouter>
    {
        public SelectActiveMarketRouterByMarketIdQuery(ulong marketId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (marketId < 1)
            {
                throw new ArgumentNullException(nameof(marketId));
            }

            MarketId = marketId;
        }

        public ulong MarketId { get; }
    }
}
