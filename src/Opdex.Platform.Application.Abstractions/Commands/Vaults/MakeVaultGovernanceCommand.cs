using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults;

/// <summary>
/// Updates a vault governance.
/// </summary>
public class MakeVaultGovernanceCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to initialize the make vault governance command.
    /// </summary>
    /// <param name="vault">The vault to persist.</param>
    /// <param name="blockHeight">The block height used when refreshing vault property values.</param>
    /// <param name="refreshUnassignedSupply">Flag to refresh the unassigned supply of the vault, default false.</param>
    /// <param name="refreshProposedSupply">Flag to refresh the proposed supply of the vault, default false.</param>
    /// <param name="refreshTotalPledgeMinimum">Flag to refresh the total pledge minimum amount, default false.</param>
    /// <param name="refreshTotalVoteMinimum">Flag to refresh the total vote minimum amount, default false.</param>
    public MakeVaultGovernanceCommand(VaultGovernance vault, ulong blockHeight, bool refreshUnassignedSupply = false, bool refreshProposedSupply = false,
                                      bool refreshTotalPledgeMinimum = false, bool refreshTotalVoteMinimum = false)
    {
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height cannot be zero.");

        Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        BlockHeight = blockHeight;
        RefreshUnassignedSupply = refreshUnassignedSupply;
        RefreshProposedSupply = refreshProposedSupply;
        RefreshTotalPledgeMinimum = refreshTotalPledgeMinimum;
        RefreshTotalVoteMinimum = refreshTotalVoteMinimum;
    }

    public VaultGovernance Vault { get; }
    public ulong BlockHeight { get; }
    public bool RefreshProposedSupply { get; }
    public bool RefreshUnassignedSupply { get; }
    public bool RefreshTotalPledgeMinimum { get; }
    public bool RefreshTotalVoteMinimum { get; }
    public bool Refresh => RefreshProposedSupply || RefreshUnassignedSupply || RefreshTotalPledgeMinimum || RefreshTotalVoteMinimum;
}
