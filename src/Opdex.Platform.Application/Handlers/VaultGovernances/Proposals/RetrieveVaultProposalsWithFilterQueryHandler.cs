using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Proposals;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Proposals;

public class RetrieveVaultProposalsWithFilterQueryHandler : IRequestHandler<RetrieveVaultProposalsWithFilterQuery, IEnumerable<VaultProposal>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalsWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultProposal>> Handle(RetrieveVaultProposalsWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectVaultProposalsWithFilterQuery(request.VaultId, request.Cursor), cancellationToken);
    }
}
