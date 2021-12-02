using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;

public class PersistVaultGovernanceCertificateCommand : IRequest<ulong>
{
    public PersistVaultGovernanceCertificateCommand(VaultGovernanceCertificate certificate)
    {
        Certificate = certificate ?? throw new ArgumentNullException(nameof(certificate)); ;
    }

    public VaultGovernanceCertificate Certificate { get; }
}
