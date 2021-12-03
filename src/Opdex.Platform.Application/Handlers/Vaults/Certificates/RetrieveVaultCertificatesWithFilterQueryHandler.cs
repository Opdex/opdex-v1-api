using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

public class RetrieveVaultCertificatesWithFilterQueryHandler : IRequestHandler<RetrieveVaultCertificatesWithFilterQuery, IEnumerable<VaultCertificate>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultCertificatesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultCertificate>> Handle(RetrieveVaultCertificatesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectVaultCertificatesWithFilterQuery(request.VaultId, request.Cursor), cancellationToken);
    }
}