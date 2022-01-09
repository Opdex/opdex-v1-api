using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;

public class PersistVaultGovernanceCommand : IRequest<ulong>
{
    public PersistVaultGovernanceCommand(VaultGovernance vault)
    {
        Vault = vault ?? throw new ArgumentNullException(nameof(vault), "Vault must be provided");
    }

    public VaultGovernance Vault { get; }
}
