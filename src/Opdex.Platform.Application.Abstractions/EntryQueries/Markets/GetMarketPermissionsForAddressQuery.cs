using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets;

public class GetMarketPermissionsForAddressQuery : IRequest<IEnumerable<MarketPermissionType>>
{
    public GetMarketPermissionsForAddressQuery(Address market, Address wallet)
    {
        if (market == Address.Empty) throw new ArgumentNullException(nameof(market), "Market address must not be empty.");
        if (wallet == Address.Empty) throw new ArgumentNullException(nameof(wallet), "Wallet address must not be empty.");

        Market = market;
        Wallet = wallet;
    }

    public Address Market { get; }
    public Address Wallet { get; }
}