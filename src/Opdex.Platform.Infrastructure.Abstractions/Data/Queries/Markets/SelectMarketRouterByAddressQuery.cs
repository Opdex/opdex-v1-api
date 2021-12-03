using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

public class SelectMarketRouterByAddressQuery : FindQuery<MarketRouter>
{
    public SelectMarketRouterByAddressQuery(Address routerAddress, bool findOrThrow = true) : base(findOrThrow)
    {
        if (routerAddress == Address.Empty)
        {
            throw new ArgumentNullException(nameof(routerAddress));
        }

        RouterAddress = routerAddress;
    }

    public Address RouterAddress { get; }
}