using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

public class RetrieveVaultContractCertificateSummaryByOwnerQueryHandler
    : IRequestHandler<RetrieveVaultContractCertificateSummaryByOwnerQuery, VaultContractCertificateSummary>
{
    private readonly IMediator _mediator;

    public RetrieveVaultContractCertificateSummaryByOwnerQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<VaultContractCertificateSummary> Handle(RetrieveVaultContractCertificateSummaryByOwnerQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new CallCirrusGetVaultContractCertificateSummaryByOwnerQuery(request.Vault,
                                                                                                     request.Owner,
                                                                                                     request.BlockHeight), cancellationToken);
    }
}
