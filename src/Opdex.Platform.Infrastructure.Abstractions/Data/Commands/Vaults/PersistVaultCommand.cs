using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;

public class PersistVaultCommand : IRequest<ulong>
{
    public PersistVaultCommand(Vault vault)
    {
        Vault = vault ?? throw new ArgumentNullException(nameof(vault), "Vault must be provided");
    }

    public Vault Vault { get; }
}
