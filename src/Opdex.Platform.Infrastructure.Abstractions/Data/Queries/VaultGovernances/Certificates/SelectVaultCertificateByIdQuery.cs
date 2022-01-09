using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;

public class SelectVaultCertificateByIdQuery : FindQuery<VaultCertificate>
{
    public SelectVaultCertificateByIdQuery(ulong certificateId, bool findOrThrow = true) : base(findOrThrow)
    {
        CertificateId = certificateId > 0 ? certificateId : throw new ArgumentOutOfRangeException(nameof(certificateId), "Certificate Id must be greater than zero.");
    }

    public ulong CertificateId { get; }
}
