using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

public class RetrieveVaultByAddressQuery : FindQuery<Vault>
{
    public RetrieveVaultByAddressQuery(Address vault, bool findOrThrow = true) : base(findOrThrow)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be set.");
    }

    public Address Vault { get; }
}