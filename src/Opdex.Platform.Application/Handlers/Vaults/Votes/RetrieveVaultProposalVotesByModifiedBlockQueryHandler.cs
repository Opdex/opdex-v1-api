using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Votes;

public class RetrieveVaultProposalVotesByModifiedBlockQueryHandler : IRequestHandler<RetrieveVaultProposalVotesByModifiedBlockQuery, IEnumerable<VaultProposalVote>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalVotesByModifiedBlockQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<IEnumerable<VaultProposalVote>> Handle(RetrieveVaultProposalVotesByModifiedBlockQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalVotesByModifiedBlockQuery(request.BlockHeight), cancellationToken);
    }
}
