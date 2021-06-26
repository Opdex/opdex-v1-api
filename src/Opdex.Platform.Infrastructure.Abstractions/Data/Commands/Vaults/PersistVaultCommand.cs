using System;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults
{
    public class PersistVaultCommand : IRequest<long>
    {
        public PersistVaultCommand(Vault vault)
        {
            Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        }

        public Vault Vault { get; }
    }
}
