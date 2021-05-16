using System;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketByIdQuery : IRequest<Market>
    {
        public RetrieveMarketByIdQuery(long marketId)
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