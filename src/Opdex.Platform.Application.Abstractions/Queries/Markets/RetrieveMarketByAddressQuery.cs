using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketByAddressQuery : FindQuery<Market>
    {
        public RetrieveMarketByAddressQuery(string address, bool findOrThrow = true) : base(findOrThrow)
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