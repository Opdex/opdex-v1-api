using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Pledges;
using Opdex.Platform.Application.Abstractions.Queries.Vaults.Proposals;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults.Pledges;

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
        var logs = new[] { TransactionLogType.VaultProposalPledgeLog, TransactionLogType.VaultProposalWithdrawPledgeLog };
        var pledges = await _mediator.Send(new RetrieveVaultProposalPledgesByModifiedBlockQuery(request.RewindHeight), CancellationToken.None);
        var pledgesList = pledges.ToList();
        var staleCount = pledgesList.Count;

        _logger.LogDebug($"Found {staleCount} stale vault proposal pledges.");

        int refreshFailureCount = 0;

        var pledgesByVault = pledgesList.GroupBy(pledge => pledge.VaultId);

        foreach (var vaultGroup in pledgesByVault)
        {
            var vault = await _mediator.Send(new RetrieveVaultByIdQuery(vaultGroup.Key), CancellationToken.None);

            var pledgesByProposal = vaultGroup.GroupBy(pledge => pledge.ProposalId);

            foreach (var pledgeGroup in pledgesByProposal)
            {
                var proposal = await _mediator.Send(new RetrieveVaultProposalByIdQuery(pledgeGroup.Key), CancellationToken.None);

                foreach (var chunk in pledgeGroup.Chunk(10))
                {
                    await Task.WhenAll(chunk.Select(async pledge =>
                    {
                        var latestPledgeTx = await _mediator.Send(new SelectTransactionForVaultProposalPledgeRewindQuery(vault.Address, pledge.Pledger,
                                                                                                                         proposal.PublicId), CancellationToken.None);

                        if (latestPledgeTx == null)
                        {
                            _logger.LogWarning($"Unable to find a more recent pledge transaction for pledge Id: {pledge.Id}");
                            return;
                        }

                        var latestLog = latestPledgeTx.LogsOfTypes(logs).OrderBy(log => log.SortOrder).First();

                        if (latestLog is VaultProposalPledgeLog pledgeLog) pledge.Update(pledgeLog, request.RewindHeight);
                        else if (latestLog is VaultProposalWithdrawPledgeLog withdrawLog) pledge.Update(withdrawLog, request.RewindHeight);

                        var pledgeId = await _mediator.Send(new MakeVaultProposalPledgeCommand(pledge, request.RewindHeight), CancellationToken.None);

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
