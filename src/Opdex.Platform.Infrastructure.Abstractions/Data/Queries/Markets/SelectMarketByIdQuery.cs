using System;
using MediatR;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectMarketByIdQuery : FindQuery<Market>
    {
        public SelectMarketByIdQuery(ulong marketId, bool findOrThrow = true) : base(findOrThrow)
        {
            if (marketId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(marketId));
            }

            MarketId = marketId;
        }

        public ulong MarketId { get; }
    }
}
