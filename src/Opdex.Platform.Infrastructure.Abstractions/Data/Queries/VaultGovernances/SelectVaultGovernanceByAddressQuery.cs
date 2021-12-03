using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;

public class SelectVaultGovernanceByAddressQuery : FindQuery<VaultGovernance>
{
    public SelectVaultGovernanceByAddressQuery(Address vault, bool findOrThrow = true) : base(findOrThrow)
    {
        Vault = vault != Address.Empty ? vault : throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
    }

    public Address Vault { get; }
}
