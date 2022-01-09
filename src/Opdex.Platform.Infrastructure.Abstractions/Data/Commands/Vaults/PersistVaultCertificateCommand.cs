using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;

public class PersistVaultCertificateCommand : IRequest<ulong>
{
    public PersistVaultCertificateCommand(VaultCertificate certificate)
    {
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate), "Vault certificate must be provided.");
    }

    public VaultCertificate Certificate { get; }
}
