using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultProposalVoteCommand : IRequest<ulong>
{
    public PersistVaultProposalVoteCommand(VaultProposalVote vote)
    {
        Vote = vote ?? throw new ArgumentNullException(nameof(vote), "Vault vote must be provided.");
    }

    public VaultProposalVote Vote { get; }
}
