using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets;

public class RetrieveMarketRouterByAddressQuery : FindQuery<MarketRouter>
{
    public RetrieveMarketRouterByAddressQuery(Address routerAddress, bool findOrThrow = true) : base(findOrThrow)
    {
        if (routerAddress == Address.Empty)
        {
            throw new ArgumentNullException(nameof(routerAddress));
        }

        RouterAddress = routerAddress;
    }

    public Address RouterAddress { get; }
}