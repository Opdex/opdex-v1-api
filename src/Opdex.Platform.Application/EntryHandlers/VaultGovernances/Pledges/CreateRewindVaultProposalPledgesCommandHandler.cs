using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.EntryCommands.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.VaultGovernances.Proposals;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.VaultGovernances.Pledges;

public class CreateRewindVaultProposalPledgesCommandHandler : IRequestHandler<CreateRewindVaultProposalPledgesCommand, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreateRewindVaultProposalPledgesCommandHandler> _logger;

    public CreateRewindVaultProposalPledgesCommandHandler(IMediator mediator, ILogger<CreateRewindVaultProposalPledgesCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(CreateRewindVaultProposalPledgesCommand request, CancellationToken cancellationToken)
    {
        var events = new[] { TransactionEventType.VaultProposalPledgeEvent, TransactionEventType.VaultProposalWithdrawPledgeEvent };
        var pledges = await _mediator.Send(new RetrieveVaultProposalPledgesByModifiedBlockQuery(request.RewindHeight));
        var pledgesList = pledges.ToList();
        var staleCount = pledgesList.Count;

        _logger.LogDebug($"Found {staleCount} stale vault proposal pledges.");

        int refreshFailureCount = 0;

        // vaultId: [{proposal: [{pledges}]}]

        // arrange pledges by vault - 1 call per vault
        var pledgesByVault = pledgesList.GroupBy(pledge => pledge.VaultGovernanceId);

        // group pledges by proposal - 1 db call per proposal
        foreach (var vaultGroup in pledgesByVault)
        {
            var vaultId = vaultGroup.Key;

            var vault = await _mediator.Send(new RetrieveVaultGovernanceByIdQuery(vaultId));

            var pledgesByProposal = vaultGroup.GroupBy(pledge => pledge.ProposalId);

            foreach (var pledgeGroup in pledgesByProposal)
            {
                var proposalId = pledgeGroup.Key;

                var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(proposalId));

                var pledgeChunks = pledgeGroup.Chunk(5);

                foreach (var chunk in pledgeChunks)
                {
                    await Task.WhenAll(chunk.Select(async pledge =>
                    {
                        var cursor = new TransactionsCursor(pledge.Pledger, events, new [] {vault.Address}, SortDirectionType.DESC, 50,
                                                            PagingDirection.Forward, default);

                        var transactions = await _mediator.Send(new RetrieveTransactionsWithFilterQuery(cursor));
                        var transactionsList = transactions.ToList();

                        VaultProposalPledgeLog pledgeLog = null;
                        VaultProposalWithdrawPledgeLog withdrawLog = null;

                        var latestTransaction = transactionsList.FirstOrDefault(tx =>
                        {
                            pledgeLog = tx
                                .LogsOfType<VaultProposalPledgeLog>(TransactionLogType.VaultProposalPledgeLog)
                                .FirstOrDefault(log => log.ProposalId == proposal.PublicId);

                            withdrawLog = tx
                                .LogsOfType<VaultProposalWithdrawPledgeLog>(TransactionLogType.VaultProposalWithdrawPledgeLog)
                                .FirstOrDefault(log => log.ProposalId == proposal.PublicId);

                            return pledgeLog != null || withdrawLog != null;
                        });

                        if (latestTransaction == null)
                        {
                            // No prior history? Handle
                        }

                        if (pledgeLog != null)
                        {
                            pledge.UpdatePledge(pledgeLog.PledgerAmount, request.RewindHeight);
                        }
                        // Nothing to do if the pledge was not withdrawn
                        else if (withdrawLog != null && withdrawLog.PledgeWithdrawn)
                        {
                            pledge.UpdatePledge(withdrawLog.PledgerAmount, request.RewindHeight);
                        }

                        // Todo this refreshes the active balance on the pledge but not the pledged amount that may have changed due to the rewind
                        // -- The only way we can access this, is to look back at our transactions
                        // -- Consider grouping pledges by pledger, then for each pledger pulling pledge transactions, then match to pledges
                        var pledgeId = await _mediator.Send(new MakeVaultProposalPledgeCommand(pledge, request.RewindHeight, refreshBalance: true));

                        var pledgeRefreshed = pledgeId > 0;

                        if (!pledgeRefreshed) refreshFailureCount++;
                    }));
                }
            }
        }

        _logger.LogDebug($"Refreshed {staleCount - refreshFailureCount} vault proposal pledges.");

        if (refreshFailureCount > 0) _logger.LogError($"Failed to refresh {refreshFailureCount} stale vault proposal pledges.");

        return refreshFailureCount == 0;
    }
}
