using System;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

public class SelectMarketByAddressQuery : FindQuery<Market>
{
    public SelectMarketByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address));
    }

    public Address Address { get; }
}