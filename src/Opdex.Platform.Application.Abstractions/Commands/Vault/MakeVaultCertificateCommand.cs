using System;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Commands.Vault
{
    public class MakeVaultCertificateCommand : IRequest<bool>
    {
        public MakeVaultCertificateCommand(VaultCertificate vaultCertificate)
        {
            VaultCertificate = vaultCertificate ?? throw new ArgumentNullException(nameof(vaultCertificate));
        }
        
        public VaultCertificate VaultCertificate { get; }
    }
}