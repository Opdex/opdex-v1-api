using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.VaultGovernances.Votes;

public class MakeVaultProposalVoteCommandHandler : IRequestHandler<MakeVaultProposalVoteCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalVoteCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultProposalVoteCommand request, CancellationToken cancellationToken)
    {
        if (request.RefreshVote)
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(request.Vote.VaultGovernanceId));
            var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(request.Vote.ProposalId));
            var summary = await _mediator.Send(new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault.Address,
                                                                                                                  proposal.PublicId,
                                                                                                                  request.Vote.Voter,
                                                                                                                  request.BlockHeight));

            request.Vote.Update(summary, request.BlockHeight);
        }

        return await _mediator.Send(new PersistVaultProposalVoteCommand(request.Vote), CancellationToken.None);
    }
}
