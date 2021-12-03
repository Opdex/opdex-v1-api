using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets;

public class GetMarketByAddressQuery : IRequest<MarketDto>
{
    public GetMarketByAddressQuery(Address marketAddress)
    {
        if (marketAddress == Address.Empty)
        {
            throw new ArgumentNullException(nameof(marketAddress), $"{nameof(marketAddress)} must be provided.");
        }

        MarketAddress = marketAddress;
    }

    public Address MarketAddress { get; }
}