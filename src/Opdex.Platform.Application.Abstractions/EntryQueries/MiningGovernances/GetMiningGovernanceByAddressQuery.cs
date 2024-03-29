using MediatR;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances;

public class GetMiningGovernanceByAddressQuery : IRequest<MiningGovernanceDto>
{
    public GetMiningGovernanceByAddressQuery(Address address)
    {
        if (address == Address.Empty)
        {
            throw new ArgumentNullException(nameof(address), $"{nameof(address)} must not be null or empty.");
        }

        Address = address;
    }

    public Address Address { get; }
}