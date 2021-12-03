using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Certificates;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Certificates;

public class RetrieveVaultContractCertificateSummariesByOwnerQueryHandler
    : IRequestHandler<RetrieveVaultContractCertificateSummariesByOwnerQuery, IEnumerable<VaultContractCertificateSummary>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultContractCertificateSummariesByOwnerQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<VaultContractCertificateSummary>> Handle(RetrieveVaultContractCertificateSummariesByOwnerQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new CallCirrusGetVaultContractCertificateSummariesByOwnerQuery(request.Vault,
                                                                                             request.Owner,
                                                                                             request.BlockHeight), cancellationToken);
    }
}