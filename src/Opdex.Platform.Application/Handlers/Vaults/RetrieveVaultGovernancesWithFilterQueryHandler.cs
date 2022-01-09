using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults;

public class RetrieveVaultGovernancesWithFilterQueryHandler : IRequestHandler<RetrieveVaultGovernancesWithFilterQuery, IEnumerable<VaultGovernance>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultGovernancesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultGovernance>> Handle(RetrieveVaultGovernancesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectVaultGovernancesWithFilterQuery(request.Cursor), cancellationToken);
    }
}
