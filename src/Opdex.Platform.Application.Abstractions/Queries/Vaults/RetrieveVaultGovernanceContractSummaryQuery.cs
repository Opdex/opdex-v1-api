using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults;

/// <summary>
/// Retrieves the summary of a vault contract, with selected retrievable properties.
/// </summary>
public class RetrieveVaultContractSummaryQuery : IRequest<VaultContractSummary>
{
    public RetrieveVaultContractSummaryQuery(Address vault, ulong blockHeight, bool includeVestingDuration = false,
                                                       bool includeUnassignedSupply = false, bool includeProposedSupply = false,
                                                       bool includeTotalPledgeMinimum = false, bool includeTotalVoteMinimum = false)
    {
        if (vault == Address.Empty) throw new ArgumentNullException(nameof(vault), "Vault address must be provided.");
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");

        Vault = vault;
        BlockHeight = blockHeight;
        IncludeVestingDuration = includeVestingDuration;
        IncludeUnassignedSupply = includeUnassignedSupply;
        IncludeProposedSupply = includeProposedSupply;
        IncludeTotalPledgeMinimum = includeTotalPledgeMinimum;
        IncludeTotalVoteMinimum = includeTotalVoteMinimum;
    }

    public Address Vault { get; }
    public ulong BlockHeight { get; }
    public bool IncludeVestingDuration { get; }
    public bool IncludeUnassignedSupply { get; }
    public bool IncludeProposedSupply { get; }
    public bool IncludeTotalPledgeMinimum { get; }
    public bool IncludeTotalVoteMinimum { get; }
}
