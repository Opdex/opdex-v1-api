using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultGovernanceCommand : IRequest<ulong>
{
    public PersistVaultGovernanceCommand(VaultGovernance vault)
    {
        Vault = vault ?? throw new ArgumentNullException(nameof(vault)); ;
    }

    public VaultGovernance Vault { get; }
}
