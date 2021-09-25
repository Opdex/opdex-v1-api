using System;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;

namespace Opdex.Platform.Application.Abstractions.Commands.Vaults
{
    /// <summary>
    /// Create a make vault certificate command to persist a certificate.
    /// </summary>
    public class MakeVaultCertificateCommand : IRequest<bool>
    {
        /// <summary>
        /// Constructor to create a make vault certificate command.
        /// </summary>
        /// <param name="vaultCertificate">The vault certificate domain model to persist.</param>
        public MakeVaultCertificateCommand(VaultCertificate vaultCertificate)
        {
            VaultCertificate = vaultCertificate ?? throw new ArgumentNullException(nameof(vaultCertificate));
        }

        public VaultCertificate VaultCertificate { get; }
    }
}
