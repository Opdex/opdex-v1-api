using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults;

/// <summary>
/// Make and persist a vault proposal, optionally refresh properties against a Cirrus node.
/// </summary>
public class MakeVaultProposalCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to initialize the make vault proposal command.
    /// </summary>
    /// <param name="proposal">The proposal to persist.</param>
    /// <param name="blockHeight">The block height to use when refreshing pledge properties prior to persistence.</param>
    /// <param name="refreshProposal">Flag to refresh the proposal.</param>
    public MakeVaultProposalCommand(VaultProposal proposal, ulong blockHeight, bool refreshProposal = false)
    {
        Proposal = proposal ?? throw new ArgumentNullException(nameof(proposal), "Vault proposal must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        RefreshProposal = refreshProposal;
    }

    public VaultProposal Proposal { get; }
    public ulong BlockHeight { get; }
    public bool RefreshProposal { get; }
}
