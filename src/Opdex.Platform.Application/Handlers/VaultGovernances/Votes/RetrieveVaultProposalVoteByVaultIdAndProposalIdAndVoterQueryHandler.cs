using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances.Votes;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Votes;

public class RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler
    : IRequestHandler<RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery, VaultProposalVote>
{
    private readonly IMediator _mediator;

    public RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQueryHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<VaultProposalVote> Handle(RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(request.VaultId, request.ProposalId, request.Voter,
                                                                                                 request.FindOrThrow), cancellationToken);
    }
}
