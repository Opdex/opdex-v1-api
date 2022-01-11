using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;

public class PersistVaultProposalPledgeCommand : IRequest<ulong>
{
    public PersistVaultProposalPledgeCommand(VaultProposalPledge pledge)
    {
        Pledge = pledge ?? throw new ArgumentNullException(nameof(pledge), "Vault pledge must be provided.");
    }

    public VaultProposalPledge Pledge { get; }
}
