using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectMarketRouterByAddressQuery : FindQuery<MarketRouter>
    {
        public SelectMarketRouterByAddressQuery(string routerAddress, bool findOrThrow = true) : base(findOrThrow)
        {
            if (!routerAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(routerAddress));
            }

            RouterAddress = routerAddress;
        }
        
        public string RouterAddress { get; }
    }
}