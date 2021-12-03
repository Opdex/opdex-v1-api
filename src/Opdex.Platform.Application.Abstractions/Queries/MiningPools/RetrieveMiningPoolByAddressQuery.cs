using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningPools;

public class RetrieveMiningPoolByAddressQuery : FindQuery<MiningPool>
{
    public RetrieveMiningPoolByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address));
        }

        Address = address;
    }

    public Address Address { get; }
}