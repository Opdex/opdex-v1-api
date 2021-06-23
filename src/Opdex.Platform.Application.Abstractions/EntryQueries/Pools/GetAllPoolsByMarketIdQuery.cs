using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.PoolDtos;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Pools
{
    public class GetAllPoolsByMarketIdQuery : IRequest<IEnumerable<LiquidityPoolDto>>
    {
        public GetAllPoolsByMarketIdQuery(string marketAddress)
        {
            if (!marketAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(marketAddress));
            }

            MarketAddress = marketAddress;

        }

        public string MarketAddress { get; }
    }
}