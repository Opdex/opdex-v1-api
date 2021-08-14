using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.LiquidityPools
{
    public class CallCirrusGetOpdexLiquidityPoolReservesQuery : IRequest<string[]>
    {
        public CallCirrusGetOpdexLiquidityPoolReservesQuery(string address)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public string Address { get; }
    }
}
