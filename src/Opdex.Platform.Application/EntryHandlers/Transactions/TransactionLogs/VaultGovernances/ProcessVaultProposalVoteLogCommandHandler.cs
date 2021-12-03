using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.VaultGovernances;

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

            vote ??= new VaultProposalVote(vault.Id, request.Log.ProposalId, request.Log.Voter, request.Log.VoteAmount,
                                           request.Log.VoterAmount, request.Log.InFavor, request.BlockHeight);

            if (request.BlockHeight < vote.ModifiedBlock)
            {
                return true;
            }

            vote.Update(request.Log, request.BlockHeight);

            var persistedProposal = await _mediator.Send(new MakeVaultProposalCommand(proposal, request.BlockHeight)) > 0;
            var persistedVote = await _mediator.Send(new MakeVaultProposalVoteCommand(vote)) > 0;

            return persistedProposal && persistedVote;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(VaultProposalVoteLog)}");

            return false;
        }
    }
}
