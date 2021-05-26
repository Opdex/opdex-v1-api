using System;
using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault
{
    public class PersistVaultCommand : IRequest<long>
    {
        public PersistVaultCommand(Domain.Models.ODX.Vault vault)
        {
            if (vault == null)
            {
                throw new ArgumentNullException(nameof(vault));
            }

            Vault = vault;
        }

        public Domain.Models.ODX.Vault Vault { get; }
    }
}