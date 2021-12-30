using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets;

/// <summary>
/// Retrieve a market response model by its contract address.
/// </summary>
public class GetMarketByAddressQuery : IRequest<MarketDto>
{
    /// <summary>
    /// Create a get market by address query.
    /// </summary>
    /// <param name="marketAddress">The address of the market to fetch.</param>
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
