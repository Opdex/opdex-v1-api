using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveMarketByAddressQuery : FindQuery<Market>
    {
        public RetrieveMarketByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
        {
            Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address));
        }

        public Address Address { get; }
    }
}
