using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

/// <summary>
/// Make and persist a vault proposal vote and optionally refresh values from a Cirrus node.
/// </summary>
public class MakeVaultProposalVoteCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to initialize the make vault proposal vote command.
    /// </summary>
    /// <param name="vote">The vote to be persisted.</param>
    /// <param name="blockHeight">The block height to use when refreshing vote properties prior to persistence.</param>
    /// <param name="refreshVote">Flag to refresh the active voted amount.</param>
    public MakeVaultProposalVoteCommand(VaultProposalVote vote, ulong blockHeight, bool refreshVote = false)
    {
        Vote = vote ?? throw new ArgumentNullException(nameof(vote), "Vault vote must be provided.");
        BlockHeight = blockHeight > 0 ? blockHeight : throw new ArgumentOutOfRangeException(nameof(blockHeight));
        RefreshVote = refreshVote;
    }

    public VaultProposalVote Vote { get; }
    public ulong BlockHeight { get; }
    public bool RefreshVote { get; }
}
