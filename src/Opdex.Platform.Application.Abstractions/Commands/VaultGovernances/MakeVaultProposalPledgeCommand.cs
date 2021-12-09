using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

/// <summary>
/// Make and persist a vault proposal pledge and optionally refresh values from a Cirrus node.
/// </summary>
public class MakeVaultProposalPledgeCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to initialize the make vault proposal pledge command.
    /// </summary>
    /// <param name="pledge">The pledge to be persisted.</param>
    /// <param name="blockHeight">The block height to use when refreshing pledge properties prior to persistence.</param>
    /// <param name="refreshBalance">Flag to refresh the active pledge balance.</param>
    public MakeVaultProposalPledgeCommand(VaultProposalPledge pledge, ulong blockHeight, bool refreshBalance = false)
    {
        Pledge = pledge ?? throw new ArgumentNullException(nameof(pledge), "Vault pledge must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        RefreshBalance = refreshBalance;
    }

    public VaultProposalPledge Pledge { get; }
    public ulong BlockHeight { get; }
    public bool RefreshBalance { get; }
}
