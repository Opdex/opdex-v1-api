using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.ProposalCertificates;

public class SelectVaultProposalCertificatesByCertificateIdQuery : IRequest<IEnumerable<VaultProposalCertificate>>
{
    public SelectVaultProposalCertificatesByCertificateIdQuery(ulong certificateId)
    {
        CertiicateId = certificateId > 0 ? certificateId : throw new ArgumentOutOfRangeException(nameof(certificateId), "Certificate Id must be greater than zero.");
    }

    public ulong CertiicateId { get; }
}
