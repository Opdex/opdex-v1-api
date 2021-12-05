using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

/// <summary>
/// Make and persist a vault certificate.
/// </summary>
public class MakeVaultGovernanceCertificateCommand : IRequest<ulong>
{
    /// <summary>
    /// Constructor to initialize the make vault governance certificate command.
    /// </summary>
    /// <param name="certificate">The certificate to be persisted.</param>
    public MakeVaultGovernanceCertificateCommand(VaultCertificate certificate)
    {
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate), "Vault certificate must be provided.");
    }

    public VaultCertificate Certificate { get; }
}
