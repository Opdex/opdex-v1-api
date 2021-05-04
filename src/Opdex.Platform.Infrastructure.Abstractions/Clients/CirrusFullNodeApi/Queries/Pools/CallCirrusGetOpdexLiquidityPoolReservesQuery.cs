using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools
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