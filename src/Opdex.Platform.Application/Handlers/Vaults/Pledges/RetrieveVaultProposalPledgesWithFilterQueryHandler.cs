using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Pledges;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Pledges;

public class RetrieveVaultProposalPledgesWithFilterQueryHandler : IRequestHandler<RetrieveVaultProposalPledgesWithFilterQuery, IEnumerable<VaultProposalPledge>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalPledgesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultProposalPledge>> Handle(RetrieveVaultProposalPledgesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectVaultProposalPledgesWithFilterQuery(request.VaultId, request.Cursor), cancellationToken);
    }
}
