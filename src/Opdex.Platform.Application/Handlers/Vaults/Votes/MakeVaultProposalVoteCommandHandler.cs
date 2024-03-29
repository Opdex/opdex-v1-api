using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults.Votes;

public class MakeVaultProposalVoteCommandHandler : IRequestHandler<MakeVaultProposalVoteCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultProposalVoteCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultProposalVoteCommand request, CancellationToken cancellationToken)
    {
        if (request.RefreshBalance)
        {
            var vault = await _mediator.Send(new RetrieveVaultByIdQuery(request.Vote.VaultId), CancellationToken.None);
            var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(request.Vote.ProposalId), CancellationToken.None);
            var summary = await _mediator.Send(new CallCirrusGetVaultProposalVoteSummaryByProposalIdAndVoterQuery(vault.Address,
                                                                                                                  proposal.PublicId,
                                                                                                                  request.Vote.Voter,
                                                                                                                  request.BlockHeight), CancellationToken.None);

            request.Vote.Update(summary, request.BlockHeight);
        }

        return await _mediator.Send(new PersistVaultProposalVoteCommand(request.Vote), CancellationToken.None);
    }
}
