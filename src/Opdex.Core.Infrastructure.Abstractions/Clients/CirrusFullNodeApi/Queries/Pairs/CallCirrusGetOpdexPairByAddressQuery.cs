using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Pairs
{
    public class CallCirrusGetOpdexPairByAddressQuery : IRequest<Pair>
    {
        public CallCirrusGetOpdexPairByAddressQuery(string address)
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