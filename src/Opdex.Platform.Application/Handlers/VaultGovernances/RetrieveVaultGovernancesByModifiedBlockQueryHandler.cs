using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances;

public class RetrieveVaultGovernancesByModifiedBlockQueryHandler : IRequestHandler<RetrieveVaultGovernancesByModifiedBlockQuery, IEnumerable<VaultGovernance>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernancesByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<VaultGovernance>> Handle(RetrieveVaultGovernancesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultGovernancesByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}
