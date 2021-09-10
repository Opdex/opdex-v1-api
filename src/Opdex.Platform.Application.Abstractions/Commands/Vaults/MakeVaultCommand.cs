using System;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults
{
    public class MakeVaultCommand : IRequest<long>
    {
        public MakeVaultCommand(Vault vault)
        {
            Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        }

        public Vault Vault { get; }
    }
}
