using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

public class RetrieveVaultCertificatesByVaultIdAndOwnerQueryHandler
    : IRequestHandler<RetrieveVaultCertificatesByVaultIdAndOwnerQuery, IEnumerable<VaultCertificate>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultCertificatesByVaultIdAndOwnerQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<IEnumerable<VaultCertificate>> Handle(RetrieveVaultCertificatesByVaultIdAndOwnerQuery request,
                                                      CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultCertificatesByVaultIdAndOwnerQuery(request.VaultId, request.Owner), cancellationToken);
    }
}
