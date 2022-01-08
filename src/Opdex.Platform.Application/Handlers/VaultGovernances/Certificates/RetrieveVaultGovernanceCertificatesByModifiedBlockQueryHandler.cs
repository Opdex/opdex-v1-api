using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;

public class RetrieveVaultGovernanceCertificatesByModifiedBlockQueryHandler
    : IRequestHandler<RetrieveVaultGovernanceCertificatesByModifiedBlockQuery, IEnumerable<VaultCertificate>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceCertificatesByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<VaultCertificate>> Handle(RetrieveVaultGovernanceCertificatesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultGovernanceCertificatesByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}
