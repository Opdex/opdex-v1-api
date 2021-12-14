using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Votes;

public class RetrieveVaultProposalVotesWithFilterQueryHandler : IRequestHandler<RetrieveVaultProposalVotesWithFilterQuery, IEnumerable<VaultProposalVote>>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalVotesWithFilterQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<IEnumerable<VaultProposalVote>> Handle(RetrieveVaultProposalVotesWithFilterQuery request, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new SelectVaultProposalVotesWithFilterQuery(request.VaultId, request.ProposalId, request.Cursor), cancellationToken);
    }
}
