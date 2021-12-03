using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalVoteCommand : IRequest<ulong>
{
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
