// unset

using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets
{
    public class GetMarketByAddressQuery : IRequest<MarketDto>
    {
        public GetMarketByAddressQuery(string marketAddress)
        {
            if (!marketAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(marketAddress), $"{nameof(marketAddress)} must be provided.");
            }

            MarketAddress = marketAddress;
        }

        public string MarketAddress { get; }
    }
}
