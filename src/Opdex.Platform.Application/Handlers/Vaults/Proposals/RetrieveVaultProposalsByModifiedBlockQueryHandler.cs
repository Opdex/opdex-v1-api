using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Proposals;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Proposals;

public class RetrieveVaultProposalsByModifiedBlockQueryHandler : IRequestHandler<RetrieveVaultProposalsByModifiedBlockQuery, IEnumerable<VaultProposal>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalsByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<VaultProposal>> Handle(RetrieveVaultProposalsByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalsByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}
