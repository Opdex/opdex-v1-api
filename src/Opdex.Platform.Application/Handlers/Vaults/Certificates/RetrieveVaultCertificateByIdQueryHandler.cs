using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

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
