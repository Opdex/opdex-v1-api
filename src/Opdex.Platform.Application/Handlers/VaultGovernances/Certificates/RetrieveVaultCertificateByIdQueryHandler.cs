using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;

public class RetrieveVaultCertificateByIdQueryHandler
    : IRequestHandler<RetrieveVaultCertificateByIdQuery, VaultCertificate>
{
    private readonly IMediator _mediator;

    public RetrieveVaultCertificateByIdQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<VaultCertificate> Handle(RetrieveVaultCertificateByIdQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultCertificateByIdQuery(request.CertificateId), cancellationToken);
    }
}
