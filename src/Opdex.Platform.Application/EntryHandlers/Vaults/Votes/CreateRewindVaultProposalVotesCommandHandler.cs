using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Votes;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Votes;

public class CreateRewindVaultProposalVotesCommandHandler : IRequestHandler<CreateRewindVaultProposalVotesCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindVaultProposalVotesCommandHandler> _logger;

    public CreateRewindVaultProposalVotesCommandHandler(IMediator mediator, ILogger<CreateRewindVaultProposalVotesCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindVaultProposalVotesCommand request, CancellationToken cancellationToken)
    {
        var logs = new[] { TransactionLogType.VaultProposalVoteLog, TransactionLogType.VaultProposalWithdrawVoteLog };
        var votes = await _mediator.Send(new RetrieveVaultProposalVotesByModifiedBlockQuery(request.RewindHeight), CancellationToken.None);
        var votesList = votes.ToList();
        var staleCount = votesList.Count;

        _logger.LogDebug($"Found {staleCount} stale vault proposal votes.");

        int refreshFailureCount = 0;

        var votesByVault = votesList.GroupBy(vote => vote.VaultId);

        foreach (var vaultGroup in votesByVault)
        {
            var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(vaultGroup.Key), CancellationToken.None);

            var votesByProposal = vaultGroup.GroupBy(vote => vote.ProposalId);

            foreach (var voteGroup in votesByProposal)
            {
                var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(voteGroup.Key), CancellationToken.None);

                foreach (var chunk in voteGroup.Chunk(10))
                {
                    await Task.WhenAll(chunk.Select(async vote =>
                    {
                        var latestVoteTx = await _mediator.Send(new SelectTransactionForVaultProposalVoteRewindQuery(vault.Address, vote.Voter,
                                                                                                                         proposal.PublicId), CancellationToken.None);

                        if (latestVoteTx == null)
                        {
                            _logger.LogWarning($"Unable to find a more recent vote transaction for vote Id: {vote.Id}");
                            return;
                        }

                        var latestLog = latestVoteTx.LogsOfTypes(logs).OrderBy(log => log.SortOrder).First();

                        if (latestLog is VaultProposalVoteLog voteLog) vote.Update(voteLog, request.RewindHeight);
                        else if (latestLog is VaultProposalWithdrawVoteLog withdrawLog) vote.Update(withdrawLog, request.RewindHeight);

                        var voteId = await _mediator.Send(new MakeVaultProposalVoteCommand(vote, request.RewindHeight), CancellationToken.None);

                        var voteRefreshed = voteId > 0;

                        if (!voteRefreshed) refreshFailureCount++;
                    }));
                }
            }
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vault proposal votes.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vault proposal votes.");

        return refreshFailureCount == 0;
    }
}
