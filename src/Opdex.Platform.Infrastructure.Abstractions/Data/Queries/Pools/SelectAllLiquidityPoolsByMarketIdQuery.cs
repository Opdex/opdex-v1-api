using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools
{
    public class SelectAllLiquidityPoolsByMarketIdQuery : IRequest<IEnumerable<LiquidityPool>>
    {
        public SelectAllLiquidityPoolsByMarketIdQuery(long marketId)
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