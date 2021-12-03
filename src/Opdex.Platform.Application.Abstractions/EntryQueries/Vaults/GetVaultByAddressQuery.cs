using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;

public class GetVaultByAddressQuery : IRequest<VaultDto>
{
    public GetVaultByAddressQuery(Address address)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
    }

    public Address Address { get; }
}