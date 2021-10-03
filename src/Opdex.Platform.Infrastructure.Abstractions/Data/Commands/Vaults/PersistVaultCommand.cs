using System;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults
{
    public class PersistVaultCommand : IRequest<ulong>
    {
        public PersistVaultCommand(Vault vault)
        {
            Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        }

        public Vault Vault { get; }
    }
}
