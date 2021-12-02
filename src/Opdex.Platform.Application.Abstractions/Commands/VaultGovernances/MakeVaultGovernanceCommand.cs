using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

/// <summary>
/// Updates a vault governance.
/// </summary>
public class MakeVaultGovernanceCommand : IRequest<ulong>
{
    public MakeVaultGovernanceCommand(VaultGovernance vault, ulong blockHeight, bool refreshUnassignedSupply = false, bool refreshProposedSupply = false,
                                      bool refreshPledgeMinimum = false, bool refreshProposalMinimum = false)
    {
        if (blockHeight == 0) throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height cannot be zero.");

        Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        BlockHeight = blockHeight;
        RefreshUnassignedSupply = refreshUnassignedSupply;
        RefreshProposedSupply = refreshProposedSupply;
        RefreshPledgeMinimum = refreshPledgeMinimum;
        RefreshProposalMinimum = refreshProposalMinimum;
    }

    public VaultGovernance Vault { get; }
    public ulong BlockHeight { get; }
    public bool RefreshProposedSupply { get; }
    public bool RefreshUnassignedSupply { get; }
    public bool RefreshPledgeMinimum { get; }
    public bool RefreshProposalMinimum { get; }
    public bool Refresh => RefreshProposedSupply || RefreshUnassignedSupply || RefreshPledgeMinimum || RefreshProposalMinimum;
}
