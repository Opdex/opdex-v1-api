using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalVoteCommand : IRequest<ulong>
{
    public MakeVaultProposalVoteCommand(VaultProposalVote vote)
    {
        Vote = vote ?? throw new ArgumentNullException(nameof(vote), "Vault vote must be provided.");
    }

    public VaultProposalVote Vote { get; }
}