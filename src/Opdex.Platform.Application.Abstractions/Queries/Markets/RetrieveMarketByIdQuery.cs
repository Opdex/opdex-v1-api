using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketByIdQuery : FindQuery<Market>
    {
        public RetrieveMarketByIdQuery(long marketId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            MarketId = marketId;
        }
        
        public long MarketId { get; }
    }
}