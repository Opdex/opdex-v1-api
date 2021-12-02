using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;

public class MakeVaultGovernanceCertificateCommand : IRequest<ulong>
{
    public MakeVaultGovernanceCertificateCommand(VaultGovernanceCertificate certificate)
    {
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate), "Vault certificate must be provided.");
    }

    public VaultGovernanceCertificate Certificate { get; }
}
