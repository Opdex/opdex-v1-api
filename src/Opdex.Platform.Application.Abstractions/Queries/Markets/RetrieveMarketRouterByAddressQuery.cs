using System;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketRouterByAddressQuery : FindQuery<MarketRouter>
    {
        public RetrieveMarketRouterByAddressQuery(string routerAddress, bool findOrThrow = true) : base(findOrThrow)
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