using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pools
{
    public class CallCirrusGetOpdexLiquidityPoolByAddressQuery : IRequest<LiquidityPool>
    {
        public CallCirrusGetOpdexLiquidityPoolByAddressQuery(string address)
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