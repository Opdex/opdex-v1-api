using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Pledges;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Pledges;

public class RetrieveVaultProposalPledgesByModifiedBlockQueryHandler
    : IRequestHandler<RetrieveVaultProposalPledgesByModifiedBlockQuery, IEnumerable<VaultProposalPledge>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalPledgesByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<VaultProposalPledge>> Handle(RetrieveVaultProposalPledgesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalPledgesByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}
