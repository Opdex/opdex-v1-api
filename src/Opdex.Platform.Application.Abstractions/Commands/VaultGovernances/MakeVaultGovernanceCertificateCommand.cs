using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultGovernanceCertificateCommand : IRequest<ulong>
{
    public MakeVaultGovernanceCertificateCommand(VaultCertificate certificate)
    {
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate), "Vault certificate must be provided.");
    }

    public VaultCertificate Certificate { get; }
}
