using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;

public class RetrieveVaultGovernanceCertificatesWithFilterQueryHandler : IRequestHandler<RetrieveVaultGovernanceCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceCertificatesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultCertificate>> Handle(RetrieveVaultGovernanceCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send( new SelectVaultGovernanceCertificatesWithFilterQuery(request.VaultId, request.Cursor), cancellationToken);
    }
}
