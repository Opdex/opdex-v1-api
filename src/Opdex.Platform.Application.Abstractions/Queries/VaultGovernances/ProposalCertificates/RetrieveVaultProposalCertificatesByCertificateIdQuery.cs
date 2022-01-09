using MediatR;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.ProposalCertificates;

public class RetrieveVaultProposalCertificatesByCertificateIdQuery : IRequest<IEnumerable<VaultProposalCertificate>>
{
    public RetrieveVaultProposalCertificatesByCertificateIdQuery(ulong certificateId)
    {
        CertiicateId = certificateId > 0 ? certificateId : throw new ArgumentOutOfRangeException(nameof(certificateId), "Certificate Id must be greater than zero.");
    }

    public ulong CertiicateId { get; }
}
