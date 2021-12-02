using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultProposalCommand : IRequest<ulong>
{
    public PersistVaultProposalCommand(VaultProposal proposal)
    {
        Proposal = proposal ?? throw new ArgumentNullException(nameof(proposal), "Vault proposal must be provided.");
    }

    public VaultProposal Proposal { get; }
}
