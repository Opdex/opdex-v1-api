using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Vaults;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;

public class RetrieveVaultCertificateByIdQuery : FindQuery<VaultCertificate>
{
    public RetrieveVaultCertificateByIdQuery(ulong certificateId, bool findOrThrow = true) : base(findOrThrow)
    {
        CertificateId = certificateId > 0 ? certificateId : throw new ArgumentOutOfRangeException(nameof(certificateId), "Certificate Id must be greater than zero.");
    }

    public ulong CertificateId { get; }
}
