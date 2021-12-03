using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

public class SelectVaultByAddressQuery : FindQuery<Vault>
{
    public SelectVaultByAddressQuery(Address vault, bool findOrThrow = true) : base(findOrThrow)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Address must be set.");
    }

    public Address Vault { get; }
}