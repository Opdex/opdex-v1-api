using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Queries.Pools
{
    public class RetrieveAllPoolsByMarketIdQuery : IRequest<IEnumerable<LiquidityPool>>
    {
        public RetrieveAllPoolsByMarketIdQuery(long marketId)
        {
            if (marketId < 1)
            {
                throw new ArgumentNullException(nameof(marketId), $"{nameof(marketId)} must be greater than 0.");
            }

            MarketId = marketId;
        }

        public long MarketId { get; }
    }
}