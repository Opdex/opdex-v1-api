using System;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Application.Abstractions.EntryQueries.Pairs
{
    public class GetPairByAddressQuery : IRequest<PairDto>
    {
        public GetPairByAddressQuery(string address)
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