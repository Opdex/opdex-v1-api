using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Vaults;

public class ProcessVaultProposalVoteLogCommandHandler : IRequestHandler<ProcessVaultProposalVoteLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessVaultProposalVoteLogCommandHandler> _logger;

    public ProcessVaultProposalVoteLogCommandHandler(IMediator mediator, ILogger<ProcessVaultProposalVoteLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessVaultProposalVoteLogCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByAddressQuery(request.Log.Contract, findOrThrow: false));
            if (vault == null) return false;

            var proposal = await _mediator.Send(new RetrieveVaultProposalByVaultIdAndPublicIdQuery(vault.Id, request.Log.ProposalId, findOrThrow: false));
            if (proposal == null) return false;

            proposal.Update(request.Log, request.BlockHeight);

            var vote = await _mediator.Send(new RetrieveVaultProposalVoteByVaultIdAndProposalIdAndVoterQuery(vault.Id, proposal.Id,
                                                                                                             request.Log.Voter,
                                                                                                             findOrThrow: false));

            if (vote == null)
            {
                vote = new VaultProposalVote(vault.Id, proposal.Id, request.Log.Voter, request.Log.VoteAmount,
                                             request.Log.VoterAmount, request.Log.InFavor, request.BlockHeight);
            }
            else
            {
                if (request.BlockHeight < vote.ModifiedBlock)
                {
                    return true;
                }

                vote.Update(request.Log, request.BlockHeight);
            }

            var persistedProposal = await _mediator.Send(new MakeVaultProposalCommand(proposal, request.BlockHeight)) > 0;
            var persistedVote = await _mediator.Send(new MakeVaultProposalVoteCommand(vote, request.BlockHeight)) > 0;

            return persistedProposal && persistedVote;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(VaultProposalVoteLog)}");

            return false;
        }
    }
}
