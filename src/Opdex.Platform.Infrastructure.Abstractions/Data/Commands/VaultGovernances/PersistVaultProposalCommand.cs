using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultProposalCommand : IRequest<ulong>
{
    public PersistVaultProposalCommand(VaultProposal proposal)
    {
        Proposal = proposal ?? throw new ArgumentNullException(nameof(proposal)); ;
    }

    public VaultProposal Proposal { get; }
}
