using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Votes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Votes;

public class RetrieveVaultProposalVotesWithFilterQueryHandler : IRequestHandler<RetrieveVaultProposalVotesWithFilterQuery, IEnumerable<VaultProposalVote>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalVotesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultProposalVote>> Handle(RetrieveVaultProposalVotesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectVaultProposalVotesWithFilterQuery(request.VaultId, request.Cursor), cancellationToken);
    }
}
