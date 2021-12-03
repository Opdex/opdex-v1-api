using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;

public class RetrieveVaultGovernanceByIdQuery : FindQuery<VaultGovernance>
{
    public RetrieveVaultGovernanceByIdQuery(ulong vaultId, bool findOrThrow = true) : base(findOrThrow)
    {
        VaultId = vaultId > 0 ? vaultId : throw new ArgumentOutOfRangeException(nameof(vaultId), "VaultId must be greater than zero.");
    }

    public ulong VaultId { get; }
}
