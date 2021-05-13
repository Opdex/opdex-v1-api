using System;
using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Vault
{
    public class MakeVaultCommand : IRequest<long>
    {
        public MakeVaultCommand(Domain.Models.ODX.Vault vault)
        {
            Vault = vault ?? throw new ArgumentNullException(nameof(vault));
        }
        
        public Domain.Models.ODX.Vault Vault { get; }
    }
}