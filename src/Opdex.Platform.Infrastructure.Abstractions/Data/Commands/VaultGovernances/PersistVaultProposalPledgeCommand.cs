using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultProposalPledgeCommand : IRequest<ulong>
{
    public PersistVaultProposalPledgeCommand(VaultProposalPledge pledge)
    {
        Pledge = pledge ?? throw new ArgumentNullException(nameof(pledge), "Vault pledge must be provided.");
    }

    public VaultProposalPledge Pledge { get; }
}