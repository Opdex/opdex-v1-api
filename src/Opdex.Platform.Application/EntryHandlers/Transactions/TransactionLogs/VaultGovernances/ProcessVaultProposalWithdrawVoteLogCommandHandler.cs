using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Votes;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.VaultGovernances;

public class ProcessVaultProposalWithdrawVoteLogCommandHandler : IRequestHandler<ProcessVaultProposalWithdrawVoteLogCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessVaultProposalWithdrawVoteLogCommandHandler> _logger;

    public ProcessVaultProposalWithdrawVoteLogCommandHandler(IMediator mediator, ILogger<ProcessVaultProposalWithdrawVoteLogCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(ProcessVaultProposalWithdrawVoteLogCommand request, CancellationToken cancellationToken)
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

            if (vote == null) return false;

            if (request.BlockHeight < vote.ModifiedBlock) return true;

            vote.Update(request.Log, request.BlockHeight);

            var persistedProposal = await _mediator.Send(new MakeVaultProposalCommand(proposal)) > 0;
            var persistedWithdraw = await _mediator.Send(new MakeVaultProposalVoteCommand(vote)) > 0;

            return persistedProposal && persistedWithdraw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failure processing {nameof(VaultProposalWithdrawVoteLog)}");

            return false;
        }
    }
}
