using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Certificates;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;

public class RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler
    : IRequestHandler<RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery, IEnumerable<VaultGovernanceCertificate>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<IEnumerable<VaultGovernanceCertificate>> Handle(RetrieveVaultGovernanceCertificatesByVaultIdAndOwnerQuery request,
                                                                CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultGovernanceCertificatesByVaultIdAndOwnerQuery(request.VaultId, request.Owner), cancellationToken);
    }
}
