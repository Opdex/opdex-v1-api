using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalCommand : IRequest<ulong>
{
    public MakeVaultProposalCommand(VaultProposal proposal)
    {
        Proposal = proposal ?? throw new ArgumentNullException(nameof(proposal));
    }

    public VaultProposal Proposal { get; }
}
