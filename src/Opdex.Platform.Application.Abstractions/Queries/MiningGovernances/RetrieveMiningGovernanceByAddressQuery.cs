using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;

public class RetrieveMiningGovernanceByAddressQuery : FindQuery<MiningGovernance>
{
    public RetrieveMiningGovernanceByAddressQuery(Address address, bool findOrThrow = true) : base(findOrThrow)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address), $"{nameof(address)} must not be null or empty.");
        }

        Address = address;
    }

    public Address Address { get; }
}