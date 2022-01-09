using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

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
