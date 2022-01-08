using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Certificates;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Certificates;

public class RetrieveVaultGovernanceContractCertificateSummaryByOwnerQueryHandler
    : IRequestHandler<RetrieveVaultGovernanceContractCertificateSummaryByOwnerQuery, VaultContractCertificateSummary>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernanceContractCertificateSummaryByOwnerQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<VaultContractCertificateSummary> Handle(RetrieveVaultGovernanceContractCertificateSummaryByOwnerQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new CallCirrusGetVaultGovernanceContractCertificateSummaryByOwnerQuery(request.Vault,
                                                                                                     request.Owner,
                                                                                                     request.BlockHeight), cancellationToken);
    }
}
