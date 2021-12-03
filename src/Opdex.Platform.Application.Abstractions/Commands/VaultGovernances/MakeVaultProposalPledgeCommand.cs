using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultProposalPledgeCommand : IRequest<ulong>
{
    public MakeVaultProposalPledgeCommand(VaultProposalPledge pledge)
    {
        Pledge = pledge ?? throw new ArgumentNullException(nameof(pledge), "Vault pledge must be provided.");
    }

    public VaultProposalPledge Pledge { get; }
}