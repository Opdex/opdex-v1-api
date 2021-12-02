using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;

/// <summary>
/// Retrieves the summary of a vault governance contract, with selected retrievable properties.
/// </summary>
public class RetrieveVaultGovernanceContractSummaryQuery : IRequest<VaultGovernanceContractSummary>
{
    public RetrieveVaultGovernanceContractSummaryQuery(Address vaultGovernance, ulong blockHeight,
                                                       bool includeUnassignedSupply = false, bool includeProposedSupply = false)
    {
        if (vaultGovernance == Address.Empty) throw new ArgumentNullException(nameof(vaultGovernance), "Vault governance address must be provided.");
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");

        VaultGovernance = vaultGovernance;
        BlockHeight = blockHeight;
        IncludeUnassignedSupply = includeUnassignedSupply;
        IncludeProposedSupply = includeProposedSupply;
    }

    public Address VaultGovernance { get; }
    public ulong BlockHeight { get; }
    public bool IncludeUnassignedSupply { get; }
    public bool IncludeProposedSupply { get; }
}
